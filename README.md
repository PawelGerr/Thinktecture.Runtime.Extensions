[![Build status](https://ci.appveyor.com/api/projects/status/04cvpwo6t3bbt7vh?svg=true)](https://ci.appveyor.com/project/PawelGerr/thinktecture-runtime-extensions)
[![Thinktecture.Runtime.Extensions](https://img.shields.io/nuget/v/Thinktecture.Runtime.Extensions.svg?maxAge=3600)](https://www.nuget.org/packages/Thinktecture.Runtime.Extensions/)

Provides some base classes like enum-like class.

Nuget: `Install-Package Thinktecture.Runtime.Extensions`

# Enum-like Class

A base class for types that are easy to implement and easy to use like .NET enumerations but more flexible.

## Features
* The key of the enumeration can be of any type (not just a number or string)
* The enumeration can provide custom key equality comparer
* The enumeration can have arbitrary properties and methods
* No public nor default constructor required
* Control over creation of *invalid* items
* An item has an indication whether it is valid or not.
	* Especially useful when fetching (invalid) data from a database or external data provider (like web services)
 	* Alternative way would be to throw an exception when trying to *deserialize* an invalid item but it turned out to be inpractical in real-worlds projects.
* Easy querying for all (valid) enumeration items
* Fast lookup for an enumeration item having the key
* The enumeration can be converted to the type of the key and vice versa by libraries that are using the [TypeConverter](https://msdn.microsoft.com/en-us/library/system.componentmodel.typeconverter) internally like [Newtonsoft.Json](https://www.newtonsoft.com/json) or model binder of [ASP.NET Core MVC / Web API](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding) (see [samples](samples))

## Example
The enumeration `ProductCategory` has a key of type `string` and has no custom members.

```
public class ProductCategory : Enum<ProductCategory>
{
	public static readonly ProductCategory Fruits = new ProductCategory("Fruits");
	public static readonly ProductCategory Dairy = new ProductCategory("Dairy");

	private ProductCategory(string key)
		: base(key)
	{
	}

	protected override ProductCategory CreateInvalid(string key)
	{
		return new ProductCategory(key);
	}
}
```

The enumeration `ProductGroup` has a key of type `int`, 2 custom properties and 1 method.

```
public class ProductGroup : Enum<ProductGroup, int>
{
	public static readonly ProductGroup Apple = new ProductGroup(1, "Apple", ProductCategory.Fruits);
	public static readonly ProductGroup Orange = new ProductGroup(2, "Orange", ProductCategory.Fruits);

	public string DisplayName { get; }
	public ProductCategory Category { get; }

	private ProductGroup(int key, string displayName, ProductCategory category)
		: base(key)
	{
		DisplayName = displayName;
		Category = category;
	}

	// custom method
	public SomeResult Do(SomeDependency dependency)
	{
		EnsureValid(); // "Do()" is not allowed for invalid items

		// do something
	
		return new SomeResult();
	}

	protected override ProductGroup CreateInvalid(int key)
	{
		// the values can be anything you like except for the key,
		// the key must not be null
		return new ProductGroup(key, "Unknown product group", ProductCategory.Get("Unknown"));
	}
}
```

## API

* Getting all valid enumeration items

```
// [Fruits, Dairy]
var categories = ProductCategory.GetAll();
```

* Getting specific enumeration item

```
/**** Key is valid ****/

// Fruits
var category = ProductCategory.Get("Fruits");

/**** Key is unknown ****/

var unknownCategory = ProductCategory.Get("Grains");
// unknownCategory.Key -> "Grains"
// unknownCategory.isValid -> false
```

* Getting specific enumeration item if a valid one exists

```
if (ProductCategory.TryGet("Fruits", out var fruits))
	logger.Information("Category {category} with TryGet found", fruits);
```

* Implicit conversion to the type of the key

```
string keyOfTheCategory = category;
```

* Ensure validity (convenience method)

```
// Throws "InvalidOperationException" if not valid
category.EnsureValid();
```

* No special handling is required for json (de)serialization as soon as the key is json-serializable

```
// "Fruits"
var json = JsonConvert.SerializeObject(category);

// Fruits
var deserializedCategory = JsonConvert.DeserializeObject<ProductCategory>(json);
```

* ... the same goes for MVC controllers. The `ProductCategory` is (de)serialized as it is.

```
[Route("api")]
public class DemoController : Controller
{
	//  http://localhost:5000/api/fruits
	
	[HttpGet("{category}")]
	public IActionResult RoundTrip(ProductCategory category)
	{
		return Json(new { ProvidedCategory = category, category.IsValid });
	}
}
```

## Implementation Guidelines and Recommendations
* All items must be `public static readonly` fields.
* The constructur should not be `public`.
* The class should be `sealed`.
* The method `CreateInvalid` must not return `null`.
* The method `CreateInvalid` should be considered as a `static` method because the keyword `this` will be `null`. 
* The `KeyEqualityComparer` may be changed once and in static constructor only. The default comparer of `Enum<TEnum, TKey>` is `EqualityComparer<TKey>.Default` and `StringComparer.OrdinalIgnoreCase` of `Enum<TEnum>`.
* The generic parameter `TEnum` must be the type of currently implementing enumeration, i.e. `class MyEnum : Enum<MyEnum>`.
* The enumeration items should be immutable, i.e. all properties/fields must be initialized in constructor
* All data required by the constructor must be known during compile time, i.e. no database lookups, http request etc. should be necessary to initialize an enumeration item.
  * If some logic requires an external dependency known at runtime only then make a method and provide it as a parameter.
  * The implementation should be as simple as possible, complex logic should be moved to its own class
