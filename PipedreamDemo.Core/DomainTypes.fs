[<AutoOpen>]
module PipedreamDemo.DomainTypes

type SlotIndex = int

type NodeIndex = int

type InputIndex = NodeIndex

type SlotAddress = NodeIndex * SlotIndex

type NodeValue = float

type InputValue = NodeValue

type Node =
    | Input
    | Output

type Link = Endpoints of SlotAddress * SlotAddress

type NodeGraph = { Nodes: Node list; Links: Link list }

type Vector = XY of float * float

type NodePosition = Vector

type GraphLayout = Positions of NodePosition list

type GraphValues = NodeValue list
