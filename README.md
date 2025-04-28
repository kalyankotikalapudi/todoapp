Approach
•	So here I have created a factory class to handle provider selection
•	Created two concrete classes Provider A and Provider B which implement ITodoProviderInterface
•	ProviderA and ProviderB have similar methods but in future different providers can use different databases, external services, or custom logic,
•	So, without changing other providers or the main application flow we can do this factory pattern

Debounce Utilization
•	Utilized debouncing as per requirement on the Todo search feature.
•	When the user types in the search box, it waits for some time before sending the search request.
•	prevents too many API calls for every single keystroke.

Issues Faced
•	Model (Todo.cs)
 Initially forgot to add a parameterless constructor, which caused a deserialization error as getting  data  in JSON formfrom the frontend.
•	CORS Issues
Noticed CORS problems when connecting Vue.js frontend and .NET backend,mainly due to http vs https mismatch.used http during local development
•	Frontend Challenges
               Faced issues showing error and success messages properly in Vue.js. Solved these issues by studying  Vue.js documentation and practical examples.

Resources Utilized
•	Vue.js Documentation
•	Entity Framework Core Documentation
•	.NET Core Documentation
•	Stack Overflow
•	GPT- for quick issues just for trobuleshoot purposes
