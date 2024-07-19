using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Util;
using Android.Content.Res;

namespace Pokedex
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText editTextName;
        Button buttonSearch;
        Button buttonClear;
        Spinner spinnerSet, spinnerRarity;
        ImageView imageViewCard;

        List<string> initialImageNames = new List<string>();
        int currentImageIndex = 0;

        System.Timers.Timer imageTimer;

        bool searchingInProgress = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            try
            {
                editTextName = FindViewById<EditText>(Resource.Id.editText1);
                buttonSearch = FindViewById<Button>(Resource.Id.button1);
                buttonClear = FindViewById<Button>(Resource.Id.button2);
                spinnerSet = FindViewById<Spinner>(Resource.Id.spinner1);
                spinnerRarity = FindViewById<Spinner>(Resource.Id.spinner2);
                imageViewCard = FindViewById<ImageView>(Resource.Id.cardImageView);

                if (editTextName != null && buttonSearch != null && buttonClear != null && spinnerSet != null && spinnerRarity != null && imageViewCard != null)
                {
                    LoadSets();
                    LoadRarities();
                    FetchInitialImageNames();

                    imageTimer = new System.Timers.Timer();
                    imageTimer.Interval = 1000;
                    imageTimer.Elapsed += ImageTimer_Elapsed;
                    imageTimer.Start();

                    buttonSearch.Click += async (sender, e) =>
                    {
                        string name = editTextName.Text.Trim();
                        if (!string.IsNullOrEmpty(name))
                        {
                            imageTimer.Stop();

                            searchingInProgress = true;

                            await SearchCard(name);

                            searchingInProgress = false;

                            if (string.IsNullOrEmpty(editTextName.Text.Trim()))
                            {
                                imageTimer.Start();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "Please enter a name to search", ToastLength.Short).Show();
                        }
                    };

                    // CLEAR
                    buttonClear.Click += (sender, e) =>
                    {
                        editTextName.Text = string.Empty;
                        DisplayInitialImages();
                        imageTimer.Start();
                    };
                }
                else
                {
                    Log.Error("MainActivity", "UI element initialization failed");
                    Toast.MakeText(this, "Failed to initialize UI elements", ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Log.Error("MainActivity", $"Exception in OnCreate: {ex.Message}");
                Toast.MakeText(this, $"Exception in OnCreate: {ex.Message}", ToastLength.Short).Show();
            }
        }

        private async void FetchInitialImageNames()
        {
            string url = "http://172.18.10.125/pokedex/display_cards.php";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        List<Card> cards = JsonConvert.DeserializeObject<List<Card>>(json);

                        initialImageNames = cards.ConvertAll(card => card.CardName.ToLower());

                        foreach (string name in initialImageNames)
                        {
                            Log.Debug("FetchInitialImageNames", $"Fetched image name: {name}");
                        }

                        DisplayInitialImages();
                    }
                    else
                    {
                        Log.Error("FetchInitialImageNames", $"Failed to fetch image names. Status code: {response.StatusCode}");
                        Toast.MakeText(this, $"Failed to fetch image names. Status code: {response.StatusCode}", ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("FetchInitialImageNames", $"Exception: {ex.Message}");
                Toast.MakeText(this, $"Exception: {ex.Message}", ToastLength.Short).Show();
            }
        }

        private async void LoadSets()
        {
            string setsUrl = "http://172.18.10.125/pokedex/get_sets.php";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(setsUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        List<string> sets = JsonConvert.DeserializeObject<List<string>>(json);

                        ArrayAdapter<string> spinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, sets);
                        spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

                        spinnerSet.Adapter = spinnerAdapter;
                    }
                    else
                    {
                        Log.Error("LoadSets", $"Response code: {response.StatusCode}");
                        Toast.MakeText(this, $"Error: {response.StatusCode}", ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("LoadSets", $"Exception: {ex.Message}");
                Toast.MakeText(this, $"Exception: {ex.Message}", ToastLength.Short).Show();
            }
        }

        private async void LoadRarities()
        {
            string raritiesUrl = "http://172.18.10.125/pokedex/get_rarities.php";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(raritiesUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        List<string> rarities = JsonConvert.DeserializeObject<List<string>>(json);

                        ArrayAdapter<string> spinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, rarities);
                        spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

                        spinnerRarity.Adapter = spinnerAdapter;
                    }
                    else
                    {
                        Log.Error("LoadRarities", $"Response code: {response.StatusCode}");
                        Toast.MakeText(this, $"Error: {response.StatusCode}", ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("LoadRarities", $"Exception: {ex.Message}");
                Toast.MakeText(this, $"Exception: {ex.Message}", ToastLength.Short).Show();
            }
        }

        private void DisplayInitialImages()
        {
            if (initialImageNames.Count > 0)
            {
                if (currentImageIndex >= initialImageNames.Count)
                {
                    currentImageIndex = 0;
                }

                string imageName = initialImageNames[currentImageIndex];
                int resourceId = Resources.GetIdentifier(imageName, "drawable", PackageName);
                if (resourceId != 0)
                {
                    imageViewCard.SetImageResource(resourceId);
                }
                else
                {
                    Log.Error("DisplayInitialImages", $"Image resource not found for image name: {imageName}");
                    Toast.MakeText(this, $"Image not found for image name: {imageName}", ToastLength.Short).Show();
                }

                currentImageIndex++;
            }
            else
            {
                Log.Warn("DisplayInitialImages", "Initial image names list is empty.");
            }
        }

        private async Task SearchCard(string name)
        {
            string searchUrl = $"https://172.18.10.125/pokedex/get_cards.php?name={Uri.EscapeDataString(name)}";

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            try
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    HttpResponseMessage response = await client.GetAsync(searchUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        List<Card> cards = JsonConvert.DeserializeObject<List<Card>>(json);

                        if (cards.Count > 0)
                        {
                            string setName = cards[0].SetName;
                            string rarity = cards[0].CardRarity;

                            int setIndex = ((ArrayAdapter<string>)spinnerSet.Adapter).GetPosition(setName);
                            if (setIndex != -1)
                            {
                                spinnerSet.SetSelection(setIndex);
                            }

                            int rarityIndex = ((ArrayAdapter<string>)spinnerRarity.Adapter).GetPosition(rarity);
                            if (rarityIndex != -1)
                            {
                                spinnerRarity.SetSelection(rarityIndex);
                            }

                            foreach (Card card in cards)
                            {
                                Toast.MakeText(this, $"Card Found!", ToastLength.Long).Show();
                                await LoadImageForCard(card);
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, "No cards found", ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "Failed to retrieve data", ToastLength.Short).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SearchCard", $"Exception: {ex.Message}");
                Toast.MakeText(this, $"Exception: {ex.Message}", ToastLength.Short).Show();
            }
        }

        private async Task LoadImageForCard(Card card)
        {
            string imageName = card.CardName.ToLower();

            int resourceId = Resources.GetIdentifier(imageName, "drawable", PackageName);

            if (resourceId != 0)
            {
                imageViewCard.SetImageResource(resourceId);
            }
            else
            {
                Log.Error("LoadImageForCard", $"Image resource not found for card: {card.CardName}");
                Toast.MakeText(this, $"Image not found for card: {card.CardName}", ToastLength.Short).Show();
            }
        }

        private void ImageTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (!searchingInProgress)
                {
                    DisplayInitialImages();
                }
            });
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            imageTimer.Stop();
            imageTimer.Dispose();
        }
    }

    public class Card
    {
        [JsonProperty("cardID")]
        public int CardID { get; set; }

        [JsonProperty("cardName")]
        public string CardName { get; set; }

        [JsonProperty("setName")]
        public string SetName { get; set; }

        [JsonProperty("cardRarity")]
        public string CardRarity { get; set; }

        [JsonProperty("imageName")]
        public string ImageName { get; set; }
    }
}