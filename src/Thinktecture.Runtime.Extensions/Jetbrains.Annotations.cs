/* MIT License

Copyright (c) 2016 JetBrains http://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using System;

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming

#pragma warning disable RCS1029 // Format binary operator on next line.

// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations
{
	/// <summary>
	/// Indicates that the value of the marked element could be <c>null</c> sometimes,
	/// so the check for <c>null</c> is necessary before its usage.
	/// </summary>
	/// <example><code>
	/// [CanBeNull] object Test() => null;
	/// 
	/// void UseTest() {
	///   var p = Test();
	///   var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
	/// }
	/// </code></example>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
		AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event |
		AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.GenericParameter)]
	internal sealed class CanBeNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that the value of the marked element could never be <c>null</c>.
	/// </summary>
	/// <example><code>
	/// [NotNull] object Foo() {
	///   return null; // Warning: Possible 'null' assignment
	/// }
	/// </code></example>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
		AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event |
		AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.GenericParameter)]
	internal sealed class NotNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Can be appplied to symbols of types derived from IEnumerable as well as to symbols of Task
	/// and Lazy classes to indicate that the value of a collection item, of the Task.Result property
	/// or of the Lazy.Value property can never be null.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
		AttributeTargets.Delegate | AttributeTargets.Field)]
	internal sealed class ItemNotNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Can be appplied to symbols of types derived from IEnumerable as well as to symbols of Task
	/// and Lazy classes to indicate that the value of a collection item, of the Task.Result property
	/// or of the Lazy.Value property can be null.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
		AttributeTargets.Delegate | AttributeTargets.Field)]
	internal sealed class ItemCanBeNullAttribute : Attribute
	{
	}
}
#pragma warning restore RCS1029 // Format binary operator on next line.
