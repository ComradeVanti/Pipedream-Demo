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

let hasInput address (Endpoints (input, _)) = input = address

let hasOutput address (Endpoints (_, output)) = output = address

let tryFindLinkWithStart address graph =
    graph.Links |> List.tryFind (hasInput address)

let tryFindLinkWithEnd address graph =
    graph.Links |> List.tryFind (hasOutput address)

let hasLinkFrom address graph =
    graph |> tryFindLinkWithStart address |> Option.isSome

let hasLinkInto address graph =
    graph |> tryFindLinkWithEnd address |> Option.isSome

let canAddLinkInto address = not << (hasLinkInto address)

let tryConnect slot1 slot2 graph =
    if graph |> canAddLinkInto slot2 then
        graph |> connect slot1 slot2
    else
        graph

let removeLink link graph = graph |> mapLinks (List.except [ link ])

let removeLinkInto address graph =
    match graph |> tryFindLinkWithEnd address with
    | Some link -> graph |> removeLink link
    | None -> graph

let addCallTo pipe graph = graph |> addNode (PipeCall pipe)
