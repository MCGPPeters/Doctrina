namespace Doctrina.Math.Applied.Learning.Evolutionary

open Doctrina
open Doctrina.Math.Applied.Probability
open Doctrina.Math.Applied.Learning
open System.Runtime.InteropServices

type GeneId = GeneId of int

type Gene<'TGene> = {
    Id: GeneId
    Alleles: 'TGene list
    Enabled: bool
}

type Locus = int

type ChromosomeId = GenomeId of int

[<NoComparison>]
[<NoEquality>]
// A Genome is the primary source of genotype information used to create
// a phenotype.
type Chromosome<'TGene> = {
    Id: ChromosomeId
    Genes: (Locus * Gene<'TGene> option) list
}

// A mate is a genome that has been assigned
// a figure of merit (according to its Objective) so that it can be selected
// to reproduce based
type Mate<'TGene, 'TMerit when 'TMerit: comparison> = {
    Chromosome: Chromosome<'TGene>
    Merit: Merit<'TMerit>
}

// A member of the mating pool
type Parent<'TGene, 'TMerit when 'TMerit: comparison> = Parent of Mate<'TGene,'TMerit>

// A population is a multiset of individuals
type Population<'TGene> = Population of Chromosome<'TGene> list

// Recombination is the production of offspring of traits that differ from those found in either parent
// Crossover is an example, which process leads to offspring having different combinations of genes from those of their parents
// In gene conversion, however,  section of genetic material is copied from one chromosome to another, without the donating chromosome being changed.
type Recombination<'TGene, 'TMerit when 'TMerit: comparison> =  Parent<'TGene, 'TMerit> -> Parent<'TGene, 'TMerit> -> Genome<'TGene>

type Crossover<'TGene, 'TMerit when 'TMerit: comparison> = Recombination<'TGene, 'TMerit>

type Mutation<'TGene> = Chromosome<'TGene> -> Chromosome<'TGene>

type Simulation<'TGene> = Experiment<Chromosome<'TGene>>

module Objective = 

    [<Measure>]
    type fitness

    type Fitness = Merit<float<fitness>>
    type Fitness<'TGene> = Objective<Mate<'TGene, Fitness>, Fitness>       

    [<Measure>]
    type novelty

    type Novelty = Merit<float<novelty>>
    type Novelty<'TGene> = Objective<Mate<'TGene, Novelty>, Novelty>

module Recombination = 

    open Objective

    let rec add gene genes = 
        List.append genes [((List.length genes) + 1, gene)]

    type Worthiness<'a> =
    | Worthy of 'a
    | Less of 'a

    type Worthiness<'TGene, 'TMerit when 'TMerit: comparison> = Mate<'TGene, 'TMerit> -> Mate<'TGene, 'TMerit> -> (Worthiness<Mate<'TGene, 'TMerit> > * Worthiness<Mate<'TGene, 'TMerit> >)
    let fittest : Worthiness<'TGene, Fitness> =
        (fun mate1 mate2 ->
            match mate1.Merit = mate2.Merit with
            | true -> (Worthy mate1, Worthy mate2)
            | _ -> match mate1.Merit > mate2.Merit with
                    | true -> (Worthy mate1, Less mate2)
                    | false -> (Less mate1, Worthy mate2))



module Algorithm = 

    let genetic (genePool: Gene<'a> list) (populationSize: int) (stopCondition: 'b -> bool) : Distribution<Genome> = 


// module Strategy = 

// module Genetic =