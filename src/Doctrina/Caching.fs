namespace Doctrina.Caching

type Outcome = Search | Fetch

type Action = Action of Outcome

