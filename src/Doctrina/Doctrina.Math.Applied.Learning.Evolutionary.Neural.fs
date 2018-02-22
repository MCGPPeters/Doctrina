namespace Doctrina.Math.Applied.Learning.Evolutionary.Neural

open Doctrina.Math.Applied.Learning.Neural
open Doctrina.Math.Applied.Learning.Evolutionary
open Doctrina.Math.Applied.Probability
open MassTransit
open Doctrina.Math.Applied.Learning.Evolutionary.Recombination
open Doctrina.Math.Applied.Probability.Distribution
open Doctrina.Math.Applied.Probability.Sampling
open Doctrina.Collections.List

type ConnectionGene = Gene<Connection>

module Neat =

   

    // let rec allign (xs: Gene<'TGene> list) (ys: Gene<'TGene> list) =
    //     let xs = List.sortBy (fun x -> x.Locus) xs
    //     let ys = List.sortBy (fun y -> y.Locus) ys
    //     match (xs, ys) with
    //     | x :: xs, y :: ys -> 
    //         match (x, y) with
    //         | (xl, yl) when xl.Locus = yl.Locus -> (Some x, Some y) :: allign xs ys
    //         | (xl, yl) when xl.Locus < yl.Locus -> (Some x, None) :: allign xs (y::ys)
    //         | (xl, yl) when xl.Locus > yl.Locus -> (None, Some y) :: allign (x::xs) ys
    //         | _ -> []
    //     | [], ys -> List.map (fun z -> (None, Some z)) ys 
    //     | xs, [] -> List.map (fun z -> (None, Some z)) xs

    let pickGene distribution = 
        let gene = distribution |> pick
        match gene with
        | Randomized(Some gene) -> 
            gene
        | _ -> None
    
    let crossover (mom:  Worthiness<Mate<'TGene, 'TMerit> >) (dad: Worthiness<Mate<'TGene, 'TMerit>>) =
        match (mom, dad) with
        | (Worthy mom, Worthy dad) | (Less mom, Less dad) -> 
          allign mom.Chromosome.Genes dad.Chromosome.Genes (fun gene -> gene.Locus)
          |> List.map (fun genePair -> 
                        match genePair with
                        // Genes on the same locus get picked at random from the 2 parents
                        | (mom, dad) -> 
                            let gene = uniform[mom; dad]
                            pickGene gene)      
        | (Worthy mom, Less dad) -> 
          allign mom.Chromosome.Genes dad.Chromosome.Genes (fun gene -> gene.Locus)
          |> List.map (fun genePair -> 
                        match genePair with
                        // Genes on the same locus get picked at random from the 2 parents
                        | (Some mom, Some dad) -> 
                            let gene = uniform[Some mom;Some dad]
                            pickGene gene
                        | (mom, _) -> 
                            // always pick moms genes
                            let gene = certainly mom
                            pickGene gene)
        | (Less mom, Worthy dad) -> 
          allign mom.Chromosome.Genes dad.Chromosome.Genes (fun gene -> gene.Locus)
          |> List.map (fun genes -> 
                        match genes with
                        // Genes on the same locus get picked at random from the 2 parents
                        | (Some mom, Some dad) -> 
                            let gene = uniform[Some mom;Some dad]
                            pickGene gene                        
                        | (_, dad) -> 
                            // Always pick dads genes
                            let gene = certainly dad
                            pickGene gene)                                                                                               

    // For every point in each Genome, where each Genome shares
    // the innovation number, the Gene is chosen randomly from
    // either parent.  If one parent has an innovation absent in
    // the other, the baby may inherit the innovation
    // if it is from the more fit parent.
    let recombine (wortiness: Recombination.Worthiness<Connection, 'TMerit>) : Recombination<Connection, 'TMerit> = 
        (fun (Parent mom) (Parent dad) ->
            let (mom, dad) = wortiness mom dad   
            let offspring = crossover mom dad |> List.choose id      
            {Id = ChromosomeId Sampling.Guid; Genes = offspring})

    module Mutation =
        let alterWeight connection weightDistribution = 
            match weightDistribution |> pick with
            | Randomized (Some weight) -> 
                Ok {connection with Weight = weight }
            | Randomized None -> Error "The distribution of weights was empty" 

        let addConnection connection chromosome =
            let genes = List.append [{ Locus = Locus (NewId.Next()); Id = GeneId Sampling.Guid; Allele = connection; Enabled = true }] chromosome.Genes        
            { chromosome with Genes = genes }      

        let rec disable (con: Connection) (genes: Gene<Connection> list) = 
                match genes with
                | [] -> []
                | x::xs when x.Allele.Id = con.Id -> { x with Enabled = false } :: (disable con xs)
                | x::xs -> x :: disable con xs     

        let addNode node connection (chromosome: Chromosome<Connection>) =
            let incomingConnection = 
                {
                    connection with 
                        Id = ConnectionId Sampling.Guid
                        Input = connection.Input
                        Output = node
                        Weight = 1.0                    
                }
            let outgoingConnection = 
                {
                    connection with 
                        Id = ConnectionId Sampling.Guid
                        Input = node    
                }

            let genes = chromosome.Genes
                            |> disable connection
                            |> List.append [{ Locus = Locus (NewId.Next()); Id = GeneId Sampling.Guid; Allele = incomingConnection; Enabled = true }]
                            |> List.append [{ Locus = Locus (NewId.Next()); Id = GeneId Sampling.Guid; Allele = outgoingConnection; Enabled = true }]   

            { chromosome with Genes = genes }           

    // module Benefit = 

    //     let novelty: Novelty<'a> = fun event ->                                   
                     
    module Speciation = 

        /// I'm ignoring the difference between disjoint and excess genes
        let distance differenceCoefficient averageWeightDifferenceCoefficient (c1: Chromosome<Connection>) (c2: Chromosome<Connection>) =
            let (nGenes, nDifferent, avgWeightDifference) = allign c1.Genes c2.Genes (fun gene -> gene.Locus)
                                                            |> List.fold (fun acc (gene1, gene2) -> 
                                                                                let (n, nDifferent, averageWeightDifference) = acc
                                                                                let n' = n+1
                                                                                let alpha = 1.0/(float n')
                                                                                let (nMismatched', target) = match gene1, gene2 with
                                                                                                                | Some x, Some y -> (nDifferent, abs (x.Allele.Weight - y.Allele.Weight))
                                                                                                                | _ -> (nDifferent + 1, averageWeightDifference)
                                                                                let error = target - averageWeightDifference
                                                                                let avgWeight' = averageWeightDifference + alpha * error                                                
                                                                                (n', nMismatched', avgWeight')) (0, 0, 0.0)
            (differenceCoefficient * (float (nDifferent / nGenes))) * (averageWeightDifferenceCoefficient * avgWeightDifference)