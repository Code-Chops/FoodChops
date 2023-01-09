using System.Globalization;
using CodeChops.FoodChops.App.Domain;
using CodeChops.FoodChops.App.Domain.Amounts;
using CodeChops.FoodChops.App.Services;
using Microsoft.JSInterop;

namespace CodeChops.FoodChops.App;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		this.Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddRazorPages();
		services.AddServerSideBlazor();
		services.AddScoped<JsInterop>();
		services.AddScoped<SoundPlayer>();

		var machineCurrency = Currency.EUR;
		var machine = new Machine(
			productStacks: new[]
			{
				new ProductStack(new(type: ProductType.Candy,   price: new(machineCurrency, 1.00m)), availablePortions: 6),
				new ProductStack(new(type: ProductType.Coffee,  price: new(machineCurrency, 1.40m)), availablePortions: 14),
				new ProductStack(new(type: ProductType.Beer,    price: new(machineCurrency, 1.80m)), availablePortions: 11),
				new ProductStack(new(type: ProductType.Soup,	price: new(machineCurrency, 1.30m)), availablePortions: 8),
			},
			horizontalProductStackCount: 2,
			availableCoinsWallet: new Wallet(
				type: WalletType.FoodChops,
				coinsWithQuantity: new Dictionary<Coin, uint?>()
				{
					[Coin.EurCent10] = 20,
					[Coin.EurCent20] = 10,
					[Coin.EurCent50] = 15,
					[Coin.Eur1] = 8,
				}),
			userInsertedCoinsWallet: new Wallet(
				type: WalletType.UserInserted,
				coinsWithQuantity: new Dictionary<Coin, uint?>(),
				currency: machineCurrency),
			theme: MachineTheme.New
		);

		services.AddScoped(_ => machine);

		var user = new User(wallet: new Wallet(
			type: WalletType.User,
			coinsWithQuantity: new Dictionary<Coin, uint?>()
			{
				[Coin.EurCent10] = null,   // The user has an indefinite amount of coins. Lucky person.
				[Coin.EurCent20] = null,
				[Coin.EurCent50] = null,
				[Coin.Eur1] = null,
			}));

		services.AddScoped(_ => user);

		services.AddLocalization(options => options.ResourcesPath = "Resources");
		var supportedCultures = new List<CultureInfo>
		{
			new("en"), new("nl")
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