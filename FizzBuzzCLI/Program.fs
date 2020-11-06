// Learn more about F# at http://fsharp.org

open System
open FizzBuzz


[<EntryPoint>]
let main _argv =
    let fizzBuzz =
        Application.create Console.ReadLine Console.WriteLine

    fizzBuzz ()
    0
