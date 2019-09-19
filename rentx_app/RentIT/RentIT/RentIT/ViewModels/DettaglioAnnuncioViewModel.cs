﻿using App.Models.Image;
using RentIT.Models.Annuncio;
using RentIT.Models.User;
using RentIT.Services;
using RentIT.Services.Foto;
using RentIT.Services.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RentIT.ViewModels
{
    public class DettaglioAnnuncioViewModel : BaseViewModel<Ad>
    {
        Ad _annuncio;
        public Ad Annuncio
        {
            get { return _annuncio; }
            set
            {
                _annuncio = value;
                OnPropertyChanged();
            }
        }

        Utente _affittuario;
        public Utente Affittuario
        {
            get { return _affittuario; }
            set
            {
                _affittuario = value;
                OnPropertyChanged();
            }
        }

        Image _immagineUtente;
        public Image ImmagineUtente
        {
            get { return _immagineUtente; }
            set
            {
                _immagineUtente = value;
                OnPropertyChanged();
            }
        }

        ObservableCollection<Image> _immagini;
        public ObservableCollection<Image> Immagini
        {
            get
            {
                return _immagini;
            }
            set
            {
                _immagini = value;
                OnPropertyChanged();
            }
        }

        readonly FotoService _fotoService;
        readonly IUserService _userService;
        public DettaglioAnnuncioViewModel(INavService navService, FotoService fotoService, UserService userService) : base(navService)
        {
            _fotoService = fotoService;
            _userService = userService;
        }

        public async override Task Init(Ad annuncio)
        {
            Annuncio = annuncio;
            /*Va tutto decommentato dopo che i metodi del db funzionano
            * Affittuario = await _userService.GetUserByIdAsync(annuncio.AffittuarioId);
            * ImmagineUtente = await getPropic();
            * var imagesModel = await _fotoService.GetAdImagesAsync(Annuncio.Id);
            * Immagini = _fotoService.creaImmagini(imagesModel);
            *
            * La riga seguente serve solo a visualizzare l'annuncio dal menu a tendina */
            Immagini = _fotoService.creaImmagini(Annuncio.Immagini);
        }

        //Metodo per prendere l'immagine profilo dell'utente dal database
        public async Task<Image> getPropic()
        {
            ImageModel foto = await _fotoService.GetUserImage(Annuncio.AffittuarioId);
            Image img = new Image();
            if (foto != null)
                img = _fotoService.fromStringToImage(foto.data);
            return img;
        }

        /*
         * Cliccando sul nome utente si apre la sua scheda
         */
        Command _infoUtenteCommand;
        public Command InfoUtenteCommand
        {
            get
            {
                return _infoUtenteCommand
                    ?? (_infoUtenteCommand = new Command(async () => await ExecuteInfoUtenteCommand()));
            }
        }


        async Task ExecuteInfoUtenteCommand()
        {
            await NavService.NavigateTo<InfoUtenteViewModel, Utente>(Affittuario);
        }

        /*
         * Comando associato al bottone "chiama"
         */
        Command _callCommand;
        public Command CallCommand
        {
            get
            {
                return _callCommand
                    ?? (_callCommand = new Command(async () => await ExecuteCallCommand()));
            }
        }

        async Task ExecuteCallCommand()
        {
            try
            {
                //Da rimuovere le virgolette
                PhoneDialer.Open("Affittuario.Numero");
            }
            catch (FeatureNotSupportedException ex)
            {
                // Phone Dialer is not supported on this device.  
            }
            catch (Exception ex)
            {
                // Other error has occurred.  
            }
        }

        /*
         * Comando associato al bottone "manda email"
         */
        Command _emailCommand;
        public Command EmailCommand
        
        {
            get
            {
                return _emailCommand
                    ?? (_emailCommand = new Command(async () => await ExecuteEmailCommand()));
                
            }
        }

        async Task ExecuteEmailCommand()
        {
            List<String> destinatario = new List<string>();
            destinatario.Add("Affittuario.Email");
            var message = new EmailMessage
            {
                To = destinatario,
            };

            await Email.ComposeAsync(message);
        }
    }
}
