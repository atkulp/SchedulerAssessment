
# General Notes
It's not production-level, and it's also barely tested. I tried to hit all the points from the write-up, but kept wanting to just do one more thing! A real solution would be split into multiple projects for better reusability and maintainability. Ideally, the api controllers, repositories, and business objects are all separate. It often doesn't end up that way in production systems, but it's a goal.

I hoped to get an actual data layer using Entity Framework and a real implemention of `ISchedulerRepository`. The quick-and-dirty method using in-memory collections is a little gross, but demonstrates what needs to happen logically.

In the absense of authentication, provider calls default to "provider" and client calls default to "client". I'm nothing it not original...

# Testing
The logic in the `DummySchedulerRepository` is probably not right. This code desperately needs unit tests, but I didn't want to go overboard. They would have been in yet another project and would have faked the DTO's to call the repository methods.

For the controller methods, I'm not a big fan of unit tests since they should really be facades. Calling them in integration tests is the better way.

Though an interactive tool rather than true unit tests, the .http testing method is much nicer than Postman for simple validation testing.

# DTO's
I like the pattern of DTO's for requests and responses, but they are overkill for single parameters. On the other hand, they are easier to amend later without changing method signatures everywhere.

DTO's (in their own project) can be wrapped up in a nice Nuget package. This includes the request/response objects for external callers.

# Controllers
In reality, controllers need to return better status code for not found, conflict, etc. Using `ActionResult` is helpful to enable the handy `Ok()`, `NotFound()`, etc. return methods, but strong-typed responses are easier for IntelliSense and documentation. Better code would include attributes to specify return codes, exceptions, and return types to make the Swagger pages nicer.

# Loose ends
Appointment isn't actually suitable for a DB entity without a default ctor. ignoring for now...

Use IIS Express startup option to see Swagger docs (weirdness from project template)

Lots of code attributes are missing that could help with authorization, EF data types (depending on data layer), validation, Swagger metadata, etc.

The `AppointmentSlot` vs `Appointment` metaphor is probably a little inconsistent in a few spots. This also doesn't account for things like cancellations, no-shows, etc.

Client/Provider ID's would probably be GUID's. Or something other than string. Forgive me.

Logging is needed all over the place... I did a few at Debug level for a bonus .25 point.

# Configuration needs
- 30-minute reservation timeout needs to be configurable.
- 24-hour advance reservation requirement needs to be configurable.
- 15-minute slot duration needs to be configurable.