namespace Math.Applied.Learning.Evolutionary.Neural

open Doctrina.Math.Applied.Learning.Neural
open Doctrina.Math.Applied.Learning.Evolutionary

type NetworkGene = NetworkGene of Gene<Network>

module Neat =

    open Doctrina.Math.Applied.Probability.Distribution
    open Doctrina.Math.Applied.Probability.Sampling

    type Genome = Genome<NetworkGene>

    // For every point in each Genome, where each Genome shares
    // the innovation number, the Gene is chosen randomly from
    // either parent.  If one parent has an innovation absent in
    // the other, the baby may inherit the innovation
    // if it is from the more fit parent.
    let recombine (worthiest: Recombination.Worthiest<NetworkGene, 'TMerit>) : Recombination<NetworkGene, 'TMerit> = 
        (fun (Parent mom) (Parent dad) ->
            let worthiest = match worthiest mom dad with
                | Some mate -> 
                    Doctrina.Collections.List.zip mom.Genome.Genes dad.Genome.Genes
                        |> List.map (fun (x, y) -> uniform [x;y] |> pick)
                | None -> match List.length mom.Genome.Genes > List.length dad.Genome.Genes with
                            | true -> mom
                            | _ -> dad



            { 
            Id = GenomeId 1
            Genes = []})

