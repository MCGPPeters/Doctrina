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

    // add a connection to a chromosome, fill the gap in loci with empty genes
    // so line up can always occur easily
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

    let rec allign (xs: (Locus * Gene<'TGene>) list) (ys: (Locus * Gene<'TGene>) list) =
        match (xs, ys) with
        | x :: xs, y :: ys -> 
            match (fst x, fst y) with
            | (xl, yl) when xl = yl -> (xl, (Some (snd x), Some (snd y))) :: allign xs ys
            | (xl, yl) when xl < yl -> (xl, (Some (snd x), None)) :: allign xs (y::ys)
            | (xl, yl) when xl > yl -> (yl, (None, Some (snd y))) :: allign (x::xs) ys
            | _ -> []
        | [], ys -> List.map (fun z -> (fst z, (None, Some (snd z)) )) ys 
        | xs, [] -> List.map (fun z -> (fst z, (None, Some (snd z)) )) xs      

    let inline allign (c: Chromosome<Connection>)(c': Chromosome<Connection>) =
        
          
                           

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


