namespace FizzBuzz

open System

[<RequireQualifiedAccess>]
module Result =
    let fromOption error =
        function
        | None -> Error error
        | Some x -> Ok x

[<RequireQualifiedAccess>]
module Option =
    let fromTryTuple =
        function
        | false, _ -> None
        | true, x -> Some x

module Parser =
    let tryParse (input: string) =
        Int32.TryParse input |> Option.fromTryTuple

module Validator =
    type ValidNumber = private ValidNumber of int

    module ValidNumber =
        let value (ValidNumber number) = number

        let ofNumber number =
            match 1 <= number && number <= 4000 with
            | false -> None
            | true -> Some(ValidNumber number)

module Evaluator =
    open Validator

    let eval number =
        [ 1 .. ValidNumber.value number ]
        |> List.map (fun n -> (n, n % 3, n % 5))
        |> List.map (function
            | (_, 0, 0) -> "FizzBuzz"
            | (_, 0, _) -> "Fizz"
            | (_, _, 0) -> "Buzz"
            | (n, _, _) -> string n)
        |> String.concat "\n"


module Domain =
    open Validator

    type NumberParser = string -> int option
    type NumberValidator = int -> ValidNumber option
    type FizzBuzzEvaluator = ValidNumber -> string

    type ParserError = NotANumber of string
    type ValidatorError = InvalidNumber of int

    type Error =
        | ParserError of ParserError
        | ValidatorError of ValidatorError

    type FizzBuzzExecutor = string -> Result<string, Error>

    let execute (parseNumber: NumberParser)
                (validateNumber: NumberValidator)
                (evaluateFizzBuzz: FizzBuzzEvaluator)
                : FizzBuzzExecutor =
        let parseNumber input =
            input
            |> parseNumber
            |> Result.fromOption (NotANumber input)
            |> Result.mapError ParserError

        let validateNumber number =
            number
            |> validateNumber
            |> Result.fromOption (InvalidNumber number)
            |> Result.mapError ValidatorError

        fun input ->
            input
            |> parseNumber
            |> Result.bind validateNumber
            |> Result.map evaluateFizzBuzz


module Application =
    open Domain
    open Validator

    type Input = unit -> string
    type Output = string -> unit

    let execute =
        Domain.execute Parser.tryParse ValidNumber.ofNumber Evaluator.eval

    let viewResult =
        function
        | Ok s -> sprintf "Here is the output:\n%s" s
        | Error (ParserError (NotANumber s)) -> sprintf "%s is not a number." s
        | Error (ValidatorError (InvalidNumber number)) ->
            sprintf "You entered %i. Please enter a number between 1 and 4000." number

    let create (input: Input) (output: Output) =
        fun () ->
            output "Please enter a number between 1 and 4000."
            input () |> execute |> viewResult |> output
            
