namespace Math.Applied.Learning.Evolutionary.Neural

open Doctrina.Math.Applied.Learning.Neural
open Doctrina.Math.Applied.Learning.Evolutionary
open Doctrina.Math.Applied.Probability
open Doctrina.Collections

type NetworkGene = NetworkGene of Gene<Network>

module Neat =

    open Doctrina.Math.Applied.Learning.Evolutionary
    open Doctrina.Math.Applied.Probability.Distribution
    open Doctrina.Math.Applied.Probability.Sampling

    let rec addConnection (connectionGene: Gene<Connection>) locus (genes: (Locus * Gene<Connection> option) list) =
        match genes with
        | [] -> [(locus, Some connectionGene)]
        | x::xs -> match x with
                    | (loc, _) when locus < loc -> 
                        (locus, Some connectionGene) :: x :: xs  
                    | (loc, _) -> 
                        let gene: Gene<Connection> option = None
                        let fill = [loc + 1 .. locus - 1] |> List.map (fun a -> (a, gene))
                        x :: List.append fill (addConnection connectionGene locus xs)

    let addConnection (connection: Gene<Connection>) locus (chromosome: Chromosome<Connection>) =
        {chromosome with Genes = (addConnection connection locus chromosome.Genes)}


    // let inline allign (Chromesome c)(Chromosome c') =
    //     let rec loop xs ys =
    //         match (xs, ys) with
    //         | x :: xs, y :: ys -> 
    //             match (x.Locus, y.Locus) with
    //             | (l, l') when l = l' -> (x, y) :: loop xs ys
    //             | (l, l') when l < l' ->  
    //     loop c.Genes c'.Genes            
                           

    // let recombine (Parent worthy) (Parent less) =
    //     Doctrina.Collections.List.zip worthy.Chromosome.Genes less.Chromosome.Genes
    //     |> List.map (fun genes -> 
    //                     match genes with
    //                     | (Some worthy, Some less) -> 
    //                         let gene = uniform[worthy;less] |> pick
    //                         let (Randomized gene) = gene
    //                         gene
    //                     | (None, _) -> None
    //                     | (Some worthy, _) -> Some worthy)                                     

    // For every point in each Genome, where each Genome shares
    // the innovation number, the Gene is chosen randomly from
    // either parent.  If one parent has an innovation absent in
    // the other, the baby may inherit the innovation
    // if it is from the more fit parent.
    let recombine (wortiness: Recombination.Worthiness<NetworkGene, 'TMerit>) : Recombination<NetworkGene, 'TMerit> = 
        (fun (Parent mom) (Parent dad) ->
            let offspring = match wortiness mom dad with
                | (Worthy mom, Worthy dad) -> 
                    //
                | (Less mom, Worthy dad) ->
                        //List.map (fun (x, y) -> uniform [x;y] |> pick)
                | (Worthy mom, Less dad) -> 
                    match List.length mom.Genome.Genes > List.length dad.Genome.Genes with
                            | true -> mom
                            | _ -> dad


