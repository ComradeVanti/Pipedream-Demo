module PipedreamDemo.GraphManagement

let emptyGraph = { Nodes = []; Links = [] }

let mapNodes mapper graph = { graph with Nodes = graph.Nodes |> mapper }

let mapLinks mapper graph = { graph with Links = graph.Links |> mapper }

let addNode node graph = graph |> mapNodes (appendItem node)

let fromNodes nodes =
    nodes
    |> List.fold (fun graph node -> graph |> addNode node) emptyGraph

let nodes graph = graph.Nodes

let nodeCount graph = graph |> nodes |> List.length

let nodeAt index graph = graph |> nodes |> List.item index

let addLink link graph = graph |> mapLinks (appendItem link)

let connect slot1 slot2 graph = graph |> addLink (Endpoints(slot1, slot2))

let hasFreeInputAt address graph =
    not
    <| (graph.Links
        |> List.exists (fun (Endpoints (_, output)) -> output = address))

let tryConnect slot1 slot2 graph =
    if graph |> hasFreeInputAt slot2 then
        graph |> connect slot1 slot2
    else
        graph
