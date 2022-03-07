using System.Globalization;
using CodeChops.VendingMachine.App.Domain;
using CodeChops.VendingMachine.App.Domain.Amounts;
using CodeChops.VendingMachine.App.Services;
using Microsoft.JSInterop;

namespace CodeChops.VendingMachine.App;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		this.Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// Normally I would use Autofac for dependency injection as it can automatically register and resolve dependencies.
	// But it would be overkill for this application, because the amount of needed injection is very small.
	//public ILifetimeScope AutofacContainer { get; private set; }

	// This method gets called by the runtime. Use this method to add services to the container.
	// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddRazorPages();
		services.AddServerSideBlazor();
		services.AddScoped<JsInterop>();
		services.AddScoped<SoundPlayer>();

		var machineCurrency = Currency.EUR;
		var machine = new Machine(
			productStacks: new[]										// Extra products can easily be added without consequence
			{
				new ProductStack(new(type: ProductType.Tea,         price: new(machineCurrency, 1.30m)), availablePortions: 10),
				new ProductStack(new(type: ProductType.Espresso,    price: new(machineCurrency, 1.80m)), availablePortions: 20),
				new ProductStack(new(type: ProductType.Juice,       price: new(machineCurrency, 1.80m)), availablePortions: 20),
				new ProductStack(new(type: ProductType.ChickenSoup, price: new(machineCurrency, 1.80m)), availablePortions: 15),
			},
			horizontalProductStackCount: 2,                             // Try to set this to 1, for example
			availableCoinsWallet: new Wallet(
				type: WalletType.VendingMachine,
				coinsWithQuantity: new Dictionary<Coin, uint?>()
				{
					[Coin.EurCent10] = 100,
					[Coin.EurCent20] = 100,
					[Coin.EurCent50] = 100,
					[Coin.Eur1] = 100,
				}),
			userInsertedCoinsWallet: new Wallet(
				type: WalletType.UserInserted,
				coinsWithQuantity: new Dictionary<Coin, uint?>(),
				currency: machineCurrency),
			color: new Random().NextDouble() < 0.5                      // Create some randomness, because: why not?
				? MachineColor.Blue
				: MachineColor.Red
		);

		services.AddScoped(serviceProvider => machine);

		var user = new User(wallet: new Wallet(
			type: WalletType.User,
			coinsWithQuantity: new Dictionary<Coin, uint?>()
			{
				[Coin.EurCent10] = null,                            // The user has an indefinite amount of coins. Lucky person.
				[Coin.EurCent20] = null,
				[Coin.EurCent50] = null,
				[Coin.Eur1] = null,
			}));

		services.AddScoped(serviceProvider => user);

		services.AddLocalization(options => options.ResourcesPath = "Resources");
		var supportedCultures = new List<CultureInfo>
		{
			new CultureInfo("en"), new CultureInfo("nl")
		};
		services.Configure<RequestLocalizationOptions>(options =>
		{
			options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
			options.SupportedUICultures = supportedCultures;
		});
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRequestLocalization();
		app.UseRouting();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapBlazorHub();
			endpoints.MapFallbackToPage("/_Host");
		});
	}
}

public static class ApplicationBuilderExtensions
{
	public static void UseBrowserLocalisation(this IApplicationBuilder app, JSRuntime jsRuntime)
	{
		var browserLocale = ((IJSInProcessRuntime)jsRuntime).Invoke<string>("blazoredLocalisation.getBrowserLocale");
		var culture = new CultureInfo(browserLocale);

		CultureInfo.CurrentCulture = culture;
		CultureInfo.CurrentUICulture = culture;
	}
}