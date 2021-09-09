module PipedreamDemo.GraphManagement

let emptyGraph = Nodes []

let addNode node (Nodes nodes) = Nodes(List.append nodes [ node ])

let fromNodes nodes =
    nodes
    |> List.fold (fun graph node -> graph |> addNode node) emptyGraph

let nodeCount (Nodes nodes) = nodes |> List.length

let nodeAt index (Nodes nodes) = nodes.[index]
