using System.Collections.Generic;

namespace VendingMachine.Helpers.Services
{
	/// <summary>
	/// The implementing service represents a single value of the given type.
	/// Such services can be looked up by RepresentedValue using a ServiceDictionary.
	/// </summary>
	public interface IServiceForValue<out TValue>
	{
		/// <summary>
		/// The value represented by this service.
		/// </summary>
		TValue RepresentedValue { get; }
	}

	/// <summary>
	/// The implementing service represents multiple values of the given type.
	/// Such services can be looked up by RepresentedValue using a ServiceDictionary.
	/// </summary>
	public interface IServiceForValues<out TValue> : IServiceForValue<TValue>
	{
		/// <summary>
		/// This property is ignored in favor of its plural counterpart.
		/// </summary>
		new TValue RepresentedValue { get; }

		/// <summary>
		/// The values represented by this service.
		/// </summary>
		IEnumerable<TValue> RepresentedValues { get; }
	}
}
