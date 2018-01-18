namespace Doctrina.Math.Applied.Optimization

module Select = 
    let ArgMax f xs = List.maxBy f xs

    let ArgMin f xs = List.minBy f xs