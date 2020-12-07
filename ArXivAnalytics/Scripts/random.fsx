open System
open System.IO
open System.Text.Json

let doStuff () = 
    let buffer = Array.zeroCreate<byte> 100
    let roSpan = new ReadOnlySpan<byte>(buffer)
    printfn "Finished creating RO Span"

doStuff()