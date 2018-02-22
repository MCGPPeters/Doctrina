namespace Doctrina.Math.Applied.Optimization

module Selection = 
    let ArgMax f xs = List.maxBy f xs

    let ArgMin f xs = List.minBy f xs