namespace Doctrina.Math.Applied.Learning.Neural

open Doctrina
open System
open Doctrina.Math.Applied.Probability.Sampling

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


type ConnectionId = ConnectionId of Randomized<Guid>

type Connection = {
    Id: ConnectionId
    Input: NodeId
    Output: NodeId
    Weight: float
}

module Connection = 
    open Doctrina.Math.Applied.Probability.Sampling

    let create id input output weightDistrubution = 
        match weightDistrubution |> pick with
        | Some (Randomized weight) -> 
            Ok {
                Id = (ConnectionId id)
                Input = input
                Output = output
                Weight = weight
            }
        | None -> Error "The distibution of wieghts was empty"        

type NetworkId = NetworkId of int

type Network = {
    Id: NetworkId    
    Nodes : Node list
    Connections : Connection list
}