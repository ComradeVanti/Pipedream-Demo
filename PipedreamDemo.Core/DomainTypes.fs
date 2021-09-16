[<AutoOpen>]
module PipedreamDemo.DomainTypes

type SlotIndex = int

type NodeIndex = int

type InputIndex = NodeIndex

type SlotAddress = NodeIndex * SlotIndex

type NodeValue = float

type InputValue = NodeValue

type Pipe = { InputCount: int; OutputCount: int; Name: string }

type Node =
    | Input
    | Output
    | PipeCall of Pipe

type Link = Endpoints of SlotAddress * SlotAddress

type NodeGraph = { Nodes: Node list; Links: Link list }

type Vector = XY of float * float

type NodePosition = Vector

type GraphLayout = Positions of NodePosition list

type GraphValues = NodeValue list
