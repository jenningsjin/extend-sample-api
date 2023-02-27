Implmenting the follow task with respect to the Extend Api.

Main Application runs from `Program.cs`. The intended interface is defined by the `Session` class in `Session.cs`
Internals and Restful API calls are grouped by functionality and 'tucked away' in `ExtendInternal/`.

Note that for ease of reference, the POD types defined in the  `DataModels` namespace are paired with their respective API Context in `ExtendInternal/`.

What I think There is TODO Next:
- Refactor all the Contexts with a base Context Class
- Handle Default Values for all the config strings better (ASP.NET has better solutions for this)
- Add Refresh Logic for the Auth - ASP.NET lets you actually register async services for this.
- Actually Formally Decided How we want to handle Exceptions:
    - More Error Handling (error codes? Exceptions etc.)
    - Proper Exception Handling 