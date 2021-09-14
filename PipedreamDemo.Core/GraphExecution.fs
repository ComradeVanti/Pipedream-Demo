module PipedreamDemo.GraphExecution

open GraphManagement
open PipedreamDemo

let run inputs graph : GraphValues = List.replicate (graph |> nodeCount) 0.
