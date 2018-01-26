namespace Doctrina.Math.Applied.Learning.Neural

open Doctrina

type NodeId = NodeId of int

type InterNeuron = {
    Id: NodeId
}

// transmit signals from the central nervous system to the effector cells 
// and are also called motor neurons.
type EfferentNeuron = {
    Id: NodeId
}

type MotorNeuron = EfferentNeuron

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

type ConnectionId = ConnectionId of int

type Connection = {
    Id: ConnectionId
    Input: NodeId
    Output: NodeId
    Weight: float
}

type NetworkId = NetworkId of int

type Network = {
    Id: NetworkId    
    Nodes : Node list
    Connections : Connection list
}