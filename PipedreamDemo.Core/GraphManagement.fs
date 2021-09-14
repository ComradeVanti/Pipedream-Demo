module PipedreamDemo.GraphManagement

let emptyGraph = { Nodes = []; Links = [] }

let mapNodes mapper graph = { graph with Nodes = graph.Nodes |> mapper }

let addNode node graph = graph |> mapNodes (appendItem node)

let fromNodes nodes =
    nodes
    |> List.fold (fun graph node -> graph |> addNode node) emptyGraph

let nodes graph = graph.Nodes

let nodeCount graph = graph |> nodes |> List.length

let nodeAt index graph = graph |> nodes |> List.item index
