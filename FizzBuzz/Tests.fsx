#load "Library.fs"
open FizzBuzz

Parser.tryParse "2"
Parser.tryParse "-2"
Parser.tryParse "abcdefg"

open Validator

ValidNumber.ofNumber 0
ValidNumber.ofNumber 5
ValidNumber.ofNumber 100
ValidNumber.ofNumber 4500

ValidNumber.ofNumber 20
|> Option.map Evaluator.eval 



