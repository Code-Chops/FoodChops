using System.Collections;

namespace VendingMachine.Helpers.Services;

/// <summary>
/// <para>
/// A collection of service implementations, indexed by their RepresentedValue(s) property.
/// Helps avoid switches, such as by getting the appropriate service for a particular enum value.
/// </para>
/// <para>
/// Usable with Dependency Injection.
/// </para>
/// <para>
/// Although normally an interface (beyond IReadOnlyCollection) should exist, using this collection type directly is much easier, with auto-completion on the constructor and generic type params.
/// </para>
/// </summary>
public class ServiceDictionary<TValue, TService> : IReadOnlyCollection<KeyValuePair<TValue, TService>>
	where TService : class, IServiceForValue<TValue>
{
	/// <summary>
	/// Statically reflect (for our TKey, TValue) all types that are concrete, implement/subclass TService, are non-generic, and have a default constructor.
	/// The current implementation looks only in the current assembly, to avoid trying to load assemblies whose DLL are not present, such as in a test.
	/// </summary>
	private static readonly IEnumerable<TService> CachedServices = AppDomain.CurrentDomain.GetAssemblies()
		.Where(assembly => assembly.FullName.StartsWith("VendingMachine"))
		.SelectMany(assembly => assembly.GetTypes())
		.Where(type => !type.IsInterface && !type.IsAbstract && typeof(TService).IsAssignableFrom(type) && !type.IsGenericType && type.GetConstructor(Type.EmptyTypes) != null)
		.Select(type => (TService)Activator.CreateInstance(type))
		.ToList();

	/// <summary>
	/// The contained services, indexed by their RepresentedValue.
	/// </summary>
	private IReadOnlyDictionary<TValue, TService> ServicesByRepresentedValue { get; }

	public int Count => this.ServicesByRepresentedValue.Count;
	public IEnumerable<TValue> Keys => this.ServicesByRepresentedValue.Keys;
	public IEnumerable<TService> Values => this.ServicesByRepresentedValue.Values;

	/// <summary>
	/// Auto constructor.
	/// Reflects (cached) all concrete types that implement/subclass TService, are non-generic, and have a default constructor.
	/// </summary>
	public ServiceDictionary()
	{
		this.ServicesByRepresentedValue = GetServiceDictionary(CachedServices);
	}

	/// <summary>
	/// Depedency Injection constructor.
	/// </summary>
	public ServiceDictionary(params TService[] services)
	{
		this.ServicesByRepresentedValue = GetServiceDictionary(services);
	}

	/// <summary>
	/// Returns the given services indexed by their values.
	/// </summary>
	private static IReadOnlyDictionary<TValue, TService> GetServiceDictionary(IEnumerable<TService> services)
	{
		var servicesByValue = services.SelectMany(GetValuesForService).ToList();
		try
		{
			return servicesByValue.ToDictionary(pair => pair.Key, pair => pair.Value);
		}
		catch (ArgumentException e) when (e.Message.Contains("same key"))
		{
			var duplicatesStrings = servicesByValue
				.GroupBy(pair => pair.Key)
				.Where(group => group.Count() > 1)
				.Select(group => $"{group.Key}: [ {String.Join(", ", group.Select(pair => pair.Value.GetType().Name))} ]");
			var duplicatesString = String.Join(", \n", duplicatesStrings);
			throw new ArgumentException($"A value must be represented by a single service: \n{duplicatesString}", e);
		}
	}

	/// <summary>
	/// Returns the value(s) represented by the given service, as key-value pairs with the service itself as the value.
	/// </summary>
	private static IEnumerable<KeyValuePair<TValue, TService>> GetValuesForService(TService service)
	{
		// A service may represent one or multiple values
		var representedValues = (service as IServiceForValues<TValue>)?.RepresentedValues ?? new[] { service.RepresentedValue };
		return representedValues.Select(value => new KeyValuePair<TValue, TService>(value, service));
	}

	/// <summary>
	/// Returns the single service that is responsible for the given value.
	/// Throws if there is none.
	/// </summary>
	public TService GetServiceForValue(TValue value, params object[] constructorArguments)
	{
	    if (!this.TryGetServiceForValue(value, out var result, constructorArguments))
		{
			var availableServices = String.Join(", ", this.ServicesByRepresentedValue.Values.Take(300).Select(service => service.GetType().Name));
			throw new ArgumentException($"Got no service for key: {value}. Available services: {availableServices}.");
		}
		return result;
	}

	/// <summary>
	/// Returns whether the given value is represented by any of the contained services. Outputs that service.
	/// </summary>
	public bool TryGetServiceForValue(TValue value, out TService service, params object[] constructorArguments)
	{
		if (this.ServicesByRepresentedValue.TryGetValue(value, out service))
		{
			if (constructorArguments.Any())
			{
				service = (TService)Activator.CreateInstance(type: service.GetType(), constructorArguments);
			}
			
			return true;
		}

		return false;
	}

	/// <summary>
	/// Returns the single service that is responsible for the given value, or null if there is none.
	/// </summary>
	public TService GetServiceOrDefaultForValue(TValue value)
	{
	    if (!this.TryGetServiceForValue(value, out var result))
		{
			result = default;
		}
		return result;
	}

	public IEnumerator<KeyValuePair<TValue, TService>> GetEnumerator() => this.ServicesByRepresentedValue.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
