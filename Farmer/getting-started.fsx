#r "nuget:Farmer"

open System
open Farmer
open Farmer.Builders

let now = DateTime.Now |> DateTimeOffset
let timestamp = now.ToUnixTimeSeconds().ToString()

let myWebApp = webApp {
    name (sprintf "FarmerApp-%s" timestamp)
}

let deployment = arm {
    location Location.EastUS
    add_resource myWebApp
}

// Create template
deployment
|> Writer.quickWrite "myFirstTemplate"

// Deploy template
deployment
|> Deploy.execute "lqdev-farmer-rg" Deploy.NoParameters



