namespace Doctrina.Math.Applied.Learning.Neural

open Doctrina

type NodeId = NeuronId of int

[<NoComparison>]
[<NoEquality>]
type InterNeuron = {
    Id: NodeId
}

[<NoComparison>]
[<NoEquality>]
// transmit signals from the central nervous system to the effector cells 
// and are also called motor neurons.
type EfferentNeuron = {
    Id: NodeId
}

type MotorNeuron = EfferentNeuron

[<NoComparison>]
[<NoEquality>]
// convey information from tissues and organs into the central nervous system 
// and are also called sensory neurons.
type AfferentNeuron = {
    Id : NodeId

}

type SensoryNeuron = AfferentNeuron

type Node = 
| EfferentNeuron of EfferentNeuron
| InterNeuron of InterNeuron
| AfferentNeuron of AfferentNeuron

type ConnectionId = SynapseId of int

[<NoComparison>]
[<NoEquality>]
type Connection = {
    SynapseId: ConnectionId
    Input: NodeId
    Output: NodeId
    Weight: float
}

type NetworkId = NetworkId of int

[<NoComparison>]
[<NoEquality>]
type Network = {
    Id: NetworkId    
    Nodes : Node list
    Synapses : Connection list
}