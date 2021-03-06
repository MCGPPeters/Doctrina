namespace Doctrina.Math.Applied.Learning.Neural

open Doctrina
open System
open Doctrina.Math.Applied.Probability.Sampling
open MassTransit

type NodeId = NodeId of Randomized<Guid>

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
| Bias of float


type ConnectionId = ConnectionId of Randomized<NewId>

type Connection = {
    Id: ConnectionId
    Input: NodeId
    Output: NodeId
    Weight: float
}

module Connection = 

    let create id input output weightDistrubution = 
        match weightDistrubution |> pick with
        | (Randomized (Some weight)) -> 
            Ok {
                Id = (ConnectionId id)
                Input = input
                Output = output
                Weight = weight
            }
        | Randomized None -> Error "The distribution of weights was empty"        

type NetworkId = NetworkId of int

type Network = {
    Id: NetworkId    
    Nodes : Node list
    Connections : Connection list
}