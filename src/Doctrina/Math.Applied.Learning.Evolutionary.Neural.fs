namespace Math.Applied.Learning.Evolutionary.Neural

open Doctrina.Math.Applied.Learning.Neural
open Doctrina.Math.Applied.Learning.Evolutionary
open Doctrina.Math.Applied.Probability
open Doctrina.Collections

type NetworkGene = NetworkGene of Gene<Network>

module Neat =

    open Doctrina.Math.Applied.Learning.Evolutionary.Recombination
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

    let rec allign (xs: (Locus * Gene<'TGene>) list) (ys: (Locus * Gene<'TGene>) list) =
        match (xs, ys) with
        | x :: xs, y :: ys -> 
            match (fst x, fst y) with
            | (xl, yl) when xl = yl -> (xl, (Some (snd x), Some (snd y))) :: allign xs ys
            | (xl, yl) when xl < yl -> (xl, (Some (snd x), None)) :: allign xs (y::ys)
            | (xl, yl) when xl > yl -> (yl, (None, Some (snd y))) :: allign (x::xs) ys
            | _ -> []
        | [], ys -> List.map (fun z -> (fst z, (None, Some (snd z)) )) ys 
        | xs, [] -> List.map (fun z -> (fst z, (None, Some(snd z)) )) xs         
    
    let crossover (mom:  Worthiness<Mate<'TGene, 'TMerit> >) (dad: Worthiness<Mate<'TGene, 'TMerit> >) =
        match (mom, dad) with
        | (Worthy mom, Worthy dad) | (Less mom, Less dad)-> 
          allign mom.Chromosome.Genes dad.Chromosome.Genes
          |> List.map (fun genes -> 
                        match genes with
                        // Genes on the same locus get picked at random from the 2 parents
                        | (l, (m, d )) -> 
                            let gene = uniform[m;d] |> pick
                            match gene with
                            | Some gene -> 
                                let (Randomized (Some gene)) = gene
                                Some (l, gene)
                            | None -> None)      
        | (Worthy mom, Less dad) -> 
          allign mom.Chromosome.Genes dad.Chromosome.Genes
          |> List.map (fun genes -> 
                        match genes with
                        // Genes on the same locus get picked at random from the 2 parents
                        | (l, (Some mom, Some dad)) -> 
                            let gene = uniform[Some mom;Some dad] |> pick
                            match gene with
                            | Some gene -> 
                                let (Randomized (Some gene)) = gene
                                Some (l, gene)
                            | _ ->  None
                        | (l, (mom, _)) -> 
                            // always pick moms genes
                            match mom with
                            | Some m -> Some (l, m)
                            | _ -> None)
        | (Less mom, Worthy dad) -> 
          allign mom.Chromosome.Genes dad.Chromosome.Genes
          |> List.map (fun genes -> 
                        match genes with
                        // Genes on the same locus get picked at random from the 2 parents
                        | (l, (Some mom, Some dad)) -> 
                            let gene = uniform[Some mom;Some dad] |> pick
                            match gene with
                            | Some gene -> 
                                let (Randomized (Some gene)) = gene
                                Some (l, gene)
                            | _ -> None
                        // Always pick dads genes                        
                        | (l, (_, dad)) -> 
                            match dad with
                            | Some d -> Some (l, d)
                            | _ -> None)                                                                                               

    // For every point in each Genome, where each Genome shares
    // the innovation number, the Gene is chosen randomly from
    // either parent.  If one parent has an innovation absent in
    // the other, the baby may inherit the innovation
    // if it is from the more fit parent.
    let recombine (wortiness: Recombination.Worthiness<NetworkGene, 'TMerit>) : Recombination<NetworkGene, 'TMerit> = 
        (fun (Parent mom) (Parent dad) ->
            let (mom, dad) = wortiness mom dad   
            let offspring = crossover mom dad |> List.choose id      
            {Id = ChromosomeId 2; Genes = offspring})


