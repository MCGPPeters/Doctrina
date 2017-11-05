using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using DeepQLearning.ConvnetSharp.Layers;
using DeepQLearning.ConvnetSharp.Trainer;
using DeepQLearning.DRLAgent;
using Environment = System.Environment;

namespace DeepQLearning
{
    public partial class Form1 : Form
    {
        private readonly string _qNetWorkFilePath = Environment.CurrentDirectory + "\\deepQnet.dat";

        private int _interval = 30;

        private bool _needToStop, _paused;
        private QAgent _qAgent;

        // worker thread
        private Thread _workerThread;

        public Form1()
        {
            InitializeComponent();

            // FiX Panel double buffering issue
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, canvas, new object[] {true});
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if (_qAgent != null)
            {
                displayBox.Text = _qAgent.DrawWorld(e.Graphics);

                switch (_qAgent.SimulationSpeed)
                {
                    case 0:
                        displayBox.Text += Environment.NewLine + "Simulation speed: Slow";
                        break;

                    case 1:
                        displayBox.Text += Environment.NewLine + "Simulation speed: Normal";
                        break;

                    case 2:
                        displayBox.Text += Environment.NewLine + "Simulation speed: Fast";
                        break;

                    case 3:
                        displayBox.Text += Environment.NewLine + "Simulation speed: Very Fast";
                        break;
                    default:
                        break;
                }
            }

            canvas.Update();
        }

        // Thread safe updating of UI
        private void UpdateUI(Panel panel)
        {
            if (_needToStop)
                return;

            if (panel.InvokeRequired)
            {
                UpdateUICallback d = UpdateUI;
                Invoke(d, panel);
            }
            else
            {
                panel.Refresh();
            }
        }

        private void BackgroundThread()
        {
            while (!_needToStop)
            {
                if (!_paused)
                {
                    _qAgent.Tick();
                    UpdateUI(canvas);
                }

                Thread.Sleep(_interval);
            }
        }

        // Delegates to enable async calls for setting controls properties
        private delegate void UpdateUICallback(Panel panel);

        #region // Button Controls

        private void StopLearning_Click(object sender, EventArgs e)
        {
            _qAgent.StopLearning();
        }

        private void startLearning_Click(object sender, EventArgs e)
        {
            if (_qAgent == null)
            {
                const int numberOfInputs = 27; // 9 eyes, each sees 3 numbers (wall, green, red thing proXimity)
                const int numberOfActions = 5; // 5 possible angles agent can turn
                const int temporalWindow = 4; // amount of temporal memory. 0 = agent lives in-the-moment :)
                const int networkSize = numberOfInputs * temporalWindow + numberOfActions * temporalWindow + numberOfInputs;

                var layerDefinitions = new List<LayerDefinition>
                {
                    new LayerDefinition {Type = "input", OutSx = 1, OutSy = 1, OutDepth = networkSize},
                    new LayerDefinition {Type = "fc", NumberOfNeurons = 96, Activation = "relu"},
                    new LayerDefinition {Type = "fc", NumberOfNeurons = 96, Activation = "relu"},
                    new LayerDefinition {Type = "fc", NumberOfNeurons = 96, Activation = "relu"},
                    new LayerDefinition {Type = "regression", NumberOfNeurons = numberOfActions}
                };

                // the value function network computes a value of taking any of the possible actions
                // given an input state. Here we specify one eXplicitly the hard way
                // but user could also equivalently instead use opt.hidden_layer_sizes = [20,20]
                // to just insert simple relu hidden layers.

                // options for the Temporal Difference learner that trains the above net
                // by backpropping the temporal difference learning rule.
                //var opt = new Options { method="sgd", learning_rate=0.01, l2_decay=0.001, momentum=0.9, batch_size=10, l1_decay=0.001 };
                var options = new Options {Method = "adadelta", L2Decay = 0.001, BatchSize = 10};

                var trainingOptions = new TrainingOptions
                {
                    TemporalWindow = temporalWindow,
                    ExperienceSize = 30000,
                    StartLearnThreshold = 1000,
                    Gamma = 0.7,
                    LearningStepsTotal = 200000,
                    LearningStepsBurnin = 3000,
                    MinimalEpsilon = 0.05,
                    EpsilonTestTime = 0.00,
                    LayerDefinitions = layerDefinitions,
                    options = options
                };

                var brain = new Brain(numberOfInputs, numberOfActions, trainingOptions, 0.0001);
                var canvasWidth = canvas.Width;
                var canvasHeight = canvas.Height;
                _qAgent = new QAgent(new DRLAgent.Environment(brain, canvasWidth, canvasHeight));
            }
            else
            {
                _qAgent.StartLearning();
            }

            if (_workerThread == null)
            {
                _workerThread = new Thread(BackgroundThread);
                _workerThread.Start();
            }
        }

        private void PauseBtn_Click(object sender, EventArgs e)
        {
            if (_paused)
            {
                PauseBtn.Text = "Pause";
                _paused = false;
            }
            else
            {
                PauseBtn.Text = "Continue";
                _paused = true;
            }
        }

        private void saveNet_Click(object sender, EventArgs e)
        {
            // Save the netwok to file
            using (var fstream = new FileStream(_qNetWorkFilePath, FileMode.Create))
            {
                new BinaryFormatter().Serialize(fstream, _qAgent);
            }

            displayBox.Text = "QNetwork saved successfully";
        }

        private void loadNet_Click(object sender, EventArgs e)
        {
            // Load the netwok from file
            using (var fstream = new FileStream(_qNetWorkFilePath, FileMode.Open))
            {
                _qAgent = new BinaryFormatter().Deserialize(fstream) as QAgent;
                _qAgent.Reinitialize();
            }

            if (_workerThread == null)
            {
                _workerThread = new Thread(BackgroundThread);
                _workerThread.Start();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _needToStop = true;

            if (_workerThread != null)
            {
                // stop worker thread
                _needToStop = true;
                while (!_workerThread.Join(100))
                    Application.DoEvents();
                _workerThread = null;
            }
        }

        private void goNormal_Click(object sender, EventArgs e)
        {
            _qAgent.SimulationSpeed = 1;
            _interval = 25;
        }

        private void goFast_Click(object sender, EventArgs e)
        {
            _qAgent.SimulationSpeed = 2;
            _interval = 10;
        }

        private void goVeryFast_Click(object sender, EventArgs e)
        {
            _qAgent.SimulationSpeed = 3;
            _interval = 0;
        }

        private void goSlow_Click(object sender, EventArgs e)
        {
            _qAgent.SimulationSpeed = 0;
            _interval = 50;
        }

        #endregion
    }
}