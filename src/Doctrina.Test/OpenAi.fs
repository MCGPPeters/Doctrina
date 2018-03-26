module OpenAi

open FSharp.Data
open FSharp.Json
open FSharp.Data.HttpRequestHeaders

module Gym = 

    module Api = 

        type Environment = JsonProvider<""" {"instance_id":"86d44247"} """>
        type DiscreteSpace = JsonProvider<""" {"info":{"n":2,"name":"Discrete"}} """>
        type Observation = JsonProvider<""" {"observation":[-0.01903912988911041,0.04691154746418939,-0.011832914308208022,-0.03568967317714903]} """>

        module Environment =

            type InstantiateEnvironmentCommand = {
                env_id: string            
            }

            type ExecuteActionCommand = {
                action: int
                render: bool            
            }        

            let create baseUrl environmentId =
                let requestBody = {env_id = environmentId}
                let body = TextRequest (Json.serialize requestBody)

                let response = Http.RequestString(baseUrl + "/v1/envs/", httpMethod = HttpMethod.Post, headers = [ ContentType HttpContentTypes.Json ], body = body) 
                Environment.Parse(response)

            let reset baseUrl instanceId = 
                let response = Http.RequestString(baseUrl + "/v1/envs/" + instanceId + "/reset/", httpMethod = HttpMethod.Post, headers = [ ContentType HttpContentTypes.Json ], body = TextRequest "") 
                Observation.Parse(response)

            let step baseUrl instanceId action render = 
                let requestBody = {
                    action = action
                    render = render
                }
                let body = TextRequest (Json.serialize requestBody)

                Http.RequestString(baseUrl + "/v1/envs/" + instanceId + "/step/", httpMethod = HttpMethod.Post, headers = [ ContentType HttpContentTypes.Json ], body = body) 

        module ActionSpace =
            let get baseUrl instanceId =
                Http.RequestString(baseUrl + "/v1/envs/" + instanceId + "/action_space/", httpMethod = HttpMethod.Get) 

        module ObservationSpace = 
            let get baseUrl instanceId = 
                Http.RequestString(baseUrl + "/v1/envs/" + instanceId + "/observation_space/", httpMethod = HttpMethod.Get) 

        module Monitor = 

            type StartCommand = {
                directory: string
                force: bool
                resume: bool
                video_callable: bool
            }

            let start baseUrl instanceId outputDirectory clearMonitorFiles resume videoCallable = 
                let requestBody = {
                    directory = outputDirectory
                    force = clearMonitorFiles
                    resume = resume
                    video_callable = videoCallable
                }
                let body = TextRequest (Json.serialize requestBody)

                Http.RequestString(baseUrl + "/v1/envs/" + instanceId + "/monitor/start/", httpMethod = HttpMethod.Post, headers = [ ContentType HttpContentTypes.Json ], body = body)  

            let stop baseUrl instanceId =
                Http.RequestString(baseUrl + "/v1/envs/" + instanceId + "/monitor/close/", httpMethod = HttpMethod.Post, headers = [ ContentType HttpContentTypes.Json ], body = TextRequest "")  

    module Environment = 

        module FrozenLake = 

            open Doctrina.Math.Applied.Learning.Reinforced

            // let create baseUrl = 
            //     let frozenLake = Api.Environment.create baseUrl "FrozenLake-v0"         

            //     {
            //         Dynamics = fun (state, action) ->
            //                         let observation = Observation.Parse(Api.Environment.step baseUrl frozenLake.InstanceId)
            //                         observation.
            //     }

                // let left = Action 0
                // let down = Action 1
                // let right = Action 2
                // let up = Action 3


