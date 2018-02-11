[![Build status](https://ci.appveyor.com/api/projects/status/04cvpwo6t3bbt7vh?svg=true)](https://ci.appveyor.com/project/PawelGerr/thinktecture-runtime-extensions)

Provides some base classes like enum-like class.

# Enum-like Class

A base class for types that are easy to implement and easy to use like .NET enumerations but more flexible.

## Features
* The key of the enumeration can be of any type (not just a number)
* The enumeration can have arbitrary properties and methods
* An item has an indication whether it is valid or not.
* Easy querying for all enumeration items
* Easy lookup for an enumeration item having the key
* The enumeration can be converted to the type of the key and vice versa by libraries that are using the [TypeConverter](https://msdn.microsoft.com/en-us/library/system.componentmodel.typeconverter) internally (like [Newtonsoft.Json](https://www.newtonsoft.com/json))

## Example
The enumeration `ProductCategory` has a key of type `string` and has no custom members.

```
public class ProductCategory : EnumClass<ProductCategory>
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

The enumeration `ProductKind` has a key of type `int` and 2 custom properties.

```
public class ProductKind : EnumClass<ProductKind, int>
{
	public static readonly ProductKind Apple = new ProductKind(1, "Apple", ProductCategory.Fruits);
	public static readonly ProductKind Orange = new ProductKind(2, "Orange", ProductCategory.Fruits);

	public string DisplayName { get; }
	public ProductCategory Category { get; }

	private ProductKind(int key, string displayName, ProductCategory category)
		: base(key)
	{
		DisplayName = displayName;
		Category = category;
	}

	protected override ProductKind CreateInvalid(int key)
	{
		return new ProductKind(key, "Unknown product kind", ProductCategory.Get("Unknown"));
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

* No special handling is required for json (de)serialization as soon as the key is json-serializable

```
// "Fruits"
var json = JsonConvert.SerializeObject(category);

// Fruits
var deserializedCategory = JsonConvert.DeserializeObject<ProductCategory>(json);
```

## Implementation Guidelines
* All items must be `public static readonly` fields.
* The constructur should not be `public`.
* The method `CreateInvalid` must not return `null`.
* The `KeyEqualityComparer` may be changed once and in static constructor only. The default comparer of `EnumClass<TEnum, TKey>` is `EqualityComparer<TKey>.Default` and `StringComparer.OrdinalIgnoreCase` of `EnumClass<TEnum>`.
* The generic parameter `TEnum` must be the type of currently implementing enumeration, i.e. `class MyEnum : EnumClass<MyEnum>`.
* The enumeration items should be immutable, i.e. all properties/fields must be initialized in constructor
* All data required by the constructor must be known during compile time, i.e. no database lookups, http request etc. should be necessary to initialize an enumeration item.
  * If some logic requires an external dependency known at runtime only then make a method and provide it as a parameter.
