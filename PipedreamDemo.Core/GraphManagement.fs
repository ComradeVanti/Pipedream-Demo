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

let removeLink link graph = graph |> mapLinks (List.except [ link ])

let removeLinkInto address graph =
    match graph |> tryFindLinkWithEnd address with
    | Some link -> graph |> removeLink link
    | None -> graph

let addCallTo pipe graph = graph |> addNode (PipeCall pipe)

let endsIn nodeIndex (Endpoints (_, (outputIndex, _))) = nodeIndex = outputIndex

let findLinksInto nodeIndex graph = graph.Links |> List.where (endsIn nodeIndex)

let getInputNodeIndex (Endpoints ((inputIndex, _), _)) = inputIndex

let findDirectDependenciesIn graph nodeIndex =
    graph
    |> findLinksInto nodeIndex
    |> List.map getInputNodeIndex
    |> List.distinct

let rec findDependenciesIn graph nodeIndex =
    let direct = nodeIndex |> findDirectDependenciesIn graph
    let indirect = direct |> List.collect (findDependenciesIn graph)
    List.append direct indirect |> List.distinct

let isSelfDependentIn graph nodeIndex =
    nodeIndex
    |> findDependenciesIn graph
    |> List.contains nodeIndex

let isCyclic graph =
    graph.Nodes
    |> List.indexed
    |> List.map fst
    |> List.exists (isSelfDependentIn graph)

let tryConnect slot1 slot2 graph =
    let connected =
        if graph |> canAddLinkInto slot2 then
            graph |> connect slot1 slot2
        else
            graph

    if connected |> isCyclic then graph else connected
