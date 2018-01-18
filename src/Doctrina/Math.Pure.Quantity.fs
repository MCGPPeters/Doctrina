namespace Doctrina.Math.Applied

open Doctrina.Math.Pure.Numbers

type Probability = Probability of ProperFraction

module Probability = 

    type Distribution< ^outcome when ^outcome: comparison > = 
        Distribution of Map<'outcome, Probability>

    type Sample< ^outcome when ^outcome: comparison > = 
        Sample of Set<'outcome>

    //module Sampling = 


