﻿
using RentIT.Services.Foto;
using RentIT;
using RentIT.Models.Immagine;
using RentIT.Services.Request;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RentIT.Services.Foto
{
    public class FotoService : IFotoService
    {

        private readonly IRequestService requestService;

        public FotoService(IRequestService requestService)
        {
            this.requestService = requestService;
        }

        /*Metodo per convertire dalla stringa base64,
         * ovvero da un'immagine del tipo restituito dal db a un'image*/
        public Image fromStringToImage(string imageBase64)
        {
            //Conversione in un array di byte
            Byte[] bytes = Convert.FromBase64String(imageBase64);
            var source = ImageSource.FromStream(() => new MemoryStream(bytes));
            //Creazione immagine
            var img = new Image { Source = source };
            return img;
        }

        /*Metodo per convertire da uno stream a una stringa base64
         * Serve ad esempio a salvare nel db un'immagine dalla galleria del telefono
         * dato che con PicturePicker viene caricata come stream*/
        public string fromStreamToString(Stream stream)
        {
            MemoryStream copy = new MemoryStream();
            stream.CopyTo(copy);
            //Conversione in un array di byte
            byte[] bytes = copy.ToArray();
            //Creazione stringa
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        /*Questo metodo prende le immagini dal db, sotto forma di lista di stringhe
         * e le rende visualizzabili sotto forma di Image
         */
        public ObservableCollection<Image> creaImmagini(List<ImageModel> imagesModel)
        {
            var Immagini = new ObservableCollection<Image>();
            foreach (ImageModel image in imagesModel)
            {
                Immagini.Add(this.fromStringToImage(image.data));
            }
            return Immagini;
        }

        //metodo per uploadare le foto di un annuncio nel db(una per volta se sono tante in una lista)
        public async Task UploadAdImagesAsync(long idAnn, string imageAsBase64)
        {
            
            var baseUri = Constants.uploadImgsAnnuncioEndpoint();
            var builder = new UriBuilder(String.Concat(baseUri, "/", idAnn));
            var uri = builder.ToString();

            var img = new ImageModel
            {
                data = imageAsBase64
            };

            await requestService.PutAsync(uri, img, AppSettings.AccessToken);
        }


        //metodo per prendere dal db le immagini relative ad un annuncio
        public async Task<List<ImageModel>> GetAdImagesAsync(long idAnn) {

            
            var baseUri = Constants.getImgsAnnuncioEndpoint();
            var builder = new UriBuilder(String.Concat(baseUri, "/", idAnn));
            var uri = builder.ToString();

            return await requestService.GetAsync<List<ImageModel>>(uri);
        }

        //metodo per uploadare l'immagine dell'utente
        public async Task UploadUserImageAsync(string imageAsBase64)
        {
            //nel caso l'immagine gia ci sia nel db bisogna fare il controllo e sostituirla anziche aggiungerla
            var builder = new UriBuilder(Constants.UserEndpointUpImage());
            var uri = builder.ToString();

            var img = new ImageModel
            {
                data = imageAsBase64
            };

            await requestService.PutAsync(uri, img, AppSettings.AccessToken);
            //await CacheHelper.RemoveFromCache(profile.PhotoUrl);  
        }


        /* Metodo per prendere dal db la foto di un utente conoscendone l'idUser che deve corrispondere con il token*/
        //qui serve l'id dell'utente e non solo il token
        public async Task<ImageModel> GetUserImage(long idUser)
        {

            var baseUri = Constants.UserEndpointGetImage();
            var builder = new UriBuilder(String.Concat(baseUri, "/", idUser));
           
            var uri = builder.ToString();

            return await requestService.GetAsync<ImageModel>(uri, AppSettings.AccessToken);
        }

        
    }
}
