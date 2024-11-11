# FuManchu

Text-templating using a Handlebars-like syntax, in your .NET Project. Tokenizer/Parser based on the Razor engine provided by Microsoft.

[![NuGet](https://img.shields.io/nuget/v/FuManchu.svg)](https://www.nuget.org/packages/FuManchu/) [![.github/workflows/main.yml](https://github.com/IngeniumSE/FuManchu/actions/workflows/main.yml/badge.svg)](https://github.com/IngeniumSE/FuManchu/actions/workflows/main.yml) [![.github/workflows/release.yml](https://github.com/IngeniumSE/FuManchu/actions/workflows/release.yml/badge.svg)](https://github.com/IngeniumSE/FuManchu/actions/workflows/release.yml)

Quick Start
-----------

First thing, install FuManchu from Nuget:

	Install-Package FuManchu

Next, add a namespace using:

	using FuManchu;

Then, you're good to go:

	Handlebars.Compile("<template-name>", "Hello {{world}}!");
	string result = Handlebars.Run("<template-name>", new { world = "World" });

There is also a shorthand:

	string result = Handlebars.CompileAndRun("<template-name>", "Hello {{world}}!", new { world = "World" });

Documentation
-------------

Documentation site is coming soon. But for now, some easy instructions are provided below.

Compiling templates
-------------------
The static `Handlebars` class provides a singleton instance of the `IHandlebarsService` type. This API is modelled on the HandlebarsJS JavaScript API, so if you are familiar with that framework, you'll be fine with FuManchu.

Let's define a template and pass it to the service:

	string template = "Hello {{name}}";
	var templateFunc = Handlebars.Compile("name", template);

The `Compile` function returns a `HandlebarTemplate` delegate which you can call, passing in your model to return you're result.

	string result = templateFunc(new { name = "Matt" });

This is equivalent to the following:

	string result = Handlebars.Run("name", new { name = "Matt" });

When you call `Compile` your template is parsed and can be executed multiple times with new data.

Block Tags
------------
Block tags are define using the `{{#tag}}{{/tag}}` syntax. There are three types of block tags; built-in tags, implicit tags and helper tags.

**Built-ins: if, elseif, else**

You can use the `if`, `elseif`, `else` tags to provide conditional logic to your templates.

	{{#if value}}
	  True
	{{/if}}

	{{#if value}}
	  True
	{{else}}
	  False
	{{/if}}

	{{#if value1}}
	  Value 1
	{{#elseif value2}}
	  Value 2
	{{else}}
	  None
	{{/if}}

We resolve the truthfulness of values using the same semantics of *truthy/falsely* logic of JavaScript, therefore, values that are `null`, or the number zero, empty enumerables and empty strings, are all considered false. Everything else is considered true.

The `{{else}}` tag can be also be written as `{{^}}` in your templates

	{{#if value}}
	  True
	{{^}}
	  False
	{{/if}}

**Built-ins: unless, else**

`unless` is the negated version of `if`. It will allow you to assume truthful values, and present output if the value is falsey.

	{{#unless value}}
	  Value is not true!
	{{/unless}}

	{{#unless value}}
	  Value is not true!
	{{else}}
	  Value was true!
	{{/unless}}

 The `{{else}}` tag can be also be written as `{{^}}` in your templates

**Built-ins: each, else**

The `each` tag allows you to iterate over enumerable objects.

	<ul>
	  {{#each items}}
	    <li>{{value}}</li>
	  {{/each}}
	</ul>

The `each` block tag creates a scope around the target model (therefore each child of `items`, above), to allow you to use the `{{value}}` expressions, where `value` is a property of a child of `items`. A more concrete example could be:

	var people = new [] { new Person() { Name = "Matt" }, new Person() { Name = "Stuart" } };
	string template = "<ul>{{#each this}}<li>{{Name}}</li>{{/each}}</ul>";
		
	// result: <ul><li>Matt</li><li>Stuart</li></ul>
	string result = Handlebars.CompileAndRun("name", template, people);

The `each` tag also supports the variables `@index`, `@first`, `@last`. If you enumerate over an `IDictionary`, you also have access to `@key`.

	var people = new [] { new Person() { Name = "Matt" }, new Person() { Name = "Stuart" } };
	string template = "<ul>{{#each this}}<li>{{@index}}: {{Name}}</li>{{/each}}</ul>";
		
	// result: <ul><li>0: Matt</li><li>1: Stuart</li></ul>
	string result = Handlebars.CompileAndRun("name", template, people);

`@index` tracks the current index of the item in the enumerable. `@first` and `@last` represent true/false values as to whether you are enumerating the first or last value in an enumerable. `@key` represents the dictionary key.

You can provided a `{{else}}` switch to provide an output when the enumerable is empty:

	{{#each items}}
	  Item {{@index}}
	{{else}}
	  No items!
	{{/each}}

 The `{{else}}` tag can be also be written as `{{^}}` in your templates

**Built-ins: with, else**
The `with` block creates a scope around the parameter argument.

	var model = new { person = new { forename = "Matt", surname = "Abbott" } };

	{{#with person}}
	  Name: {{forename}} {{surname}}
	{{/with}}

Again, as with `each`, you can use the `{{else}}` switch to provide an output if the value passed into the `with` tag is falsey:

	{{#with person}}
	  Name: {{forename}} {{surname}}
	{{else}}
	  Nobody :-(
	{{/with}}

 The `{{else}}` tag can be also be written as `{{^}}` in your templates

Implicit Block Tags
-

You can use shorthand `{{#tag}}{{/tag}}` where "tag" is the name of a property on your model, instead of using full tags, e.g.

	var model = new { person = new { forename = "Matt", surname = "Abbott" } };

	{{#person}}
	  Name: {{forename}} {{surname}}
	{{/person}}

The above is equivalent to:

	{{#if person}}
	  {{#with person}}
	    Name: {{forename}} {{surname}}
	  {{/with}}
	{{/if}}

If you're property also happens to be an enumerable, then the implicit block tag works like `each`:

	{{#people}}
	  <li>{{@index}}: {{forename}} {{surname}}</li>
	{{/people}}

Inverted Block Tags
-
Inverted block tags follow the rules for implicit block tags, but are used to provided content when the tag expression resolves to *falsey*.

	{{^power}}
	  You have no power here!
	{{/power}}

Block Helpers
-

You can register custom helpers using the `Handlebars` service. You need to register a helper ahead of time, which you can then call from your template.

	Handlebars.RegisterHelper("list", options => {
	  var enumerable = options.Data as IEnumerable ?? new[] { (object)options.Data };

	  return "<ul>"
	    + string.Join("", enumerable.OfType<object>().Select(options.Fn))
	    + "</ul>";
	});

For block helpers, the `options` parameter provides access to the content of your block helper, therefore given the following usage:

	{{#list people}}
	  <li>{{forename}} {{surname}}</li>
	{{/list}}

When calling `options.Fn` (or `options.Render`), the content of your custom helper block is rendered, scoped to the value passed to the render function.

**Arguments and Hash parameters**
You can pass additional information to your helpers using your helper block, e.g.:

	var model = new {
	  people = new List<People>(),
	  message = "Hello World"
	};

	Handlebars.RegisterHelper("list", options => {
	  var people = options.Data as List<People>;
	  string message = options.Arguments[1] as string;
	  string cssClass = options.Hash["class"];

	  return "<ul class=\"" + cssClass + "\">"
	    + "<li>" + message + "</li>"
	    + string.Join("", enumerable.OfType<object>().Select(options.Fn))
	    + "</ul>";
	});

	{{#list people message class="nav nav-pills"}}...{{/list}}

An instance of `HelperOptions` is passed as the value of `options`, which provides the input arguments (`people` and `message`) and a has (`IDictionary<string, object>`, `class="nav nav-pills"`) which are accessible. `options.Data` provides a shorthand access to `options.Arguments[0]` as `dynamic`, and `options.Hash` provides readonly access to `options.Parameters`. Both `options.Data` and `options.Hash` and provided for API compatability with HandlebarsJS.

Expression Tags
-
Expression tags are simple `{{value}}` type tags that allow you to render content into your templates, by binding values from your input models (or 'contexts' in HandlebarsJS speak). There are a variety of ways you can call these properties, so given:

	var model = new {
	  person = new {
		  forename = "Matt",
	    surname = "Abbott",
	    age = 30,
	    job = new {
	      title = "Developer"
	    }
	  }
	};

	{{person.forename}}
	{{./person.forename}}
	{{this.person.forename}}
	{{this/person/forename}}
	{{@root.person.forename}}

	And also within scopes:

	{{#with person}}
	  {{#with job}}
	    {{../forename}} {{../surname}}
	  {{/with}}
	{{/with}}

You can use these same 'context paths' as arguments and hash parameters in your block tags too:

	{{#if ../people}}
	{{#with @root.people}}

And with your custom helpers too:

	{{#list people class=@root.cssClass}}

Expression Helpers
-
Just like Block Helpers, you can create Expression Helpers too, using the same function as before:

	Handlebars.RegisterHelper("name", options => {
	  return string.Format("{0} {1}", options.Data.forename, options.data.surname);
	});

Called using the expression syntax, this time with arguments:

	var model = new { forename = "Matt", surname = "Abbott" };

	{{name this}}

Partial Templates
-
Partial templates allow you to break your Handlebars templates into discreet units. To register a partial, you call the `Handlebars.RegisterPartial` method

	Handlebars.RegisterPartial("name", "{{forename}} {{surname}}");

You can then call your partial from your template:

	var model = new { forename = "Matt", surname = "Abbott" };

	{{>name}}

You can override the model passed to your template, by providing an additional arguments:

	var model = new { person = new { forename = "Matt", surname = "Abbott" } };

	{{>name person}}

You can alternatively pass through parameters, if you need to provide a parameter-like experience:

	var model = new { person = new { forename = "Matt", surname = "Abbott" } };

	{{>name firstName=person.forename lastName=person.surname}}

Partial Template Blocks
-
You can also create partial templates that allow you to insert content from the outer template into the partial template. This is useful for creating layouts or component-like partials.

	Handlebars.RegisterPartial("heading", "<h1>Hello {{<content}}</h1>");

Alternatively, you can use the HandlebarsJS syntax:

	Handlebars.RegisterPartial("heading", "<h1>Hello {{>@partial-block}}</h1>");

To call this component, you can now specify the content to be inserted into the partial template:

	{{>heading}}World{{/heading}}

Which should render:

	<h1>Hello World</h1>

You can also define partials that support multiple zones:

	Handlebars.RegisterPartial("layout", "<div>{{<header}}</div><div>{{<content}}</div><div>{{<footer}}</div>");

Which you can call as:

	{{#>layout}}
	  {>header}}Header{{/header}}
	  {{>content}}Content{{/content}}
	  {{>footer}}Footer{{/footer}}
	{{/layout}}}

Text Encoding
-
Like HandlebarsJS, FuManchu encodes values by default, therefore all calls, such as `{{forename}}` etc, will be encoded. If you need to render the raw value, you can use the triple-brace syntax:

	{{{forename}}} {{{surname}}}

Singleton vs Instance Services
-
The `Handlebars` type provides singleton access to an instance of `IHandlebarsService`. If you need instance access instead (such as injection through IoC/DI), you can simply register `HandlebarsService` as an instance of `IHandlebarsService` in whatever appropriate lifetime scope you require. This will enable you to partition your helpers/partials and compiled templates per instances of `IHandlebarsService`.
