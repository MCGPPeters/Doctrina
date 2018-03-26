// #load @"..\Doctrina\Doctrina.Collections.NonEmpty.fs"
// #load @"..\Doctrina\Doctrina.fs"

// #load @"..\Doctrina\Doctrina.Math.Applied.Probability.fs"
// #load @"..\Doctrina\Doctrina.Math.Pure.Algebra.Structure.fs"
// #load @"..\Doctrina\Doctrina.Math.Applied.Learning.fs"
// #load @"..\Doctrina\Doctrina.Math.Applied.Learning.Reinforced.fs"
#r @"C:\Users\maurice.peters\.nuget\packages\fsharp.data\2.4.5\lib\net45\FSharp.Data.dll"
#r @"C:\Users\maurice.peters\.nuget\packages\fsharp.json\0.3.1\lib\net45\FSharp.Json.dll"
#load @".\OpenAi.fs"


open OpenAi

let baseUrl = "http://EINL59683:5000"

let environmentId = "FrozenLake-v0"

let environment = Gym.Api.Environment.create baseUrl environmentId
Gym.Api.ActionSpace.get baseUrl environment.InstanceId

Gym.Api.ObservationSpace.get baseUrl environment.InstanceId




Gym.Api.Monitor.start baseUrl environment.InstanceId "c:\\tmp\\gym" true false false

Gym.Api.Environment.reset baseUrl environment.InstanceId
Gym.Api.Environment.step baseUrl environment.InstanceId 3 true
Gym.Api.Environment.step baseUrl environment.InstanceId 3 true
Gym.Api.Environment.step baseUrl environment.InstanceId 0 true
Gym.Api.Environment.step baseUrl environment.InstanceId 0 true
Gym.Api.Environment.step baseUrl environment.InstanceId 1 true
Gym.Api.Environment.step baseUrl environment.InstanceId 1 true
Gym.Api.Environment.step baseUrl environment.InstanceId 2 true
Gym.Api.Environment.step baseUrl environment.InstanceId 2 true

Gym.Api.Monitor.stop baseUrl environment.InstanceId
