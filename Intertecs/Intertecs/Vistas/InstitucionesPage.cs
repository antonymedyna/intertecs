﻿using Intertecs.Modelos;
using Intertecs.WebServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Intertecs.Vistas
{
    class InstitucionesPage : ContentPage
    {
        private ListView lv_inst;
        private SearchBar sr_inst;
        private ActivityIndicator ai_inst;
        private Label lb_ninguno, lb_seguir,lb_seleccionar;
        private BoxView bv_div;
        private List<Institucion> list_inst; 
        private WSInstitution objWsInst;
        private StackLayout st_inst;

        public InstitucionesPage()
        {
            objWsInst = new WSInstitution();
            createGUI();
        }

        private void createGUI()
        {
            NavigationPage.SetHasNavigationBar(this, false); //Quitar barra de navegacion 
            ai_inst = new ActivityIndicator()
            {
                Color = Color.Green,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            sr_inst = new SearchBar()
            {
                HorizontalTextAlignment = TextAlignment.Start,
                Placeholder = "Buscar Institución",
                FontSize = 16,
                TextColor = Color.Black
            };
            sr_inst.TextChanged += (sender, e) => buscarInstitucion(sr_inst.Text);
            DataTemplate celda = new DataTemplate(typeof(ImageCell));
            celda.SetBinding(TextCell.TextProperty, "institution");
            celda.SetValue(TextCell.TextColorProperty,Color.Gray);
            celda.SetBinding(TextCell.DetailProperty, "short_name");
            celda.SetValue(TextCell.TextColorProperty, Color.Blue);
            celda.SetBinding(ImageCell.ImageSourceProperty, "logo");

            lv_inst = new ListView()
            {
                HasUnevenRows = true, //Estandarizar items
                ItemTemplate = celda
            };
            lv_inst.ItemSelected += (sender, e) =>
             {

                 Institucion objIns = (Institucion)e.SelectedItem;
                 
                 Settings.Settings.institucionName = objIns.institution;
                 Settings.Settings.institucionShortName = objIns.short_name;
                 Settings.Settings.institucionLogo = objIns.logo;

                 DisplayAlert("Itemselected", Settings.Settings.institucionName+"\n"+
                     Settings.Settings.institucionShortName + "\n" 
                     + Settings.Settings.institucionLogo + "\n", "Aceptar");
                 //Agregar con settings 
                 App.Current.MainPage = new DashBoard();
             };
            bv_div = new BoxView()
            {
                Color               = Color.Green,
                HeightRequest       = 2,
                HorizontalOptions   = LayoutOptions.Center,
                WidthRequest        = 300
            };
            lb_ninguno = new Label()
            {
                HorizontalTextAlignment= TextAlignment.Center,
                Text= "Ninguno",
                TextColor = Color.Gray,
                FontFamily ="Roboto"
            };
            var tapr = new TapGestureRecognizer();
            tapr.Tapped += (sender, e) =>
            {
                DisplayAlert("error","selecciono inguno","aceptar");
                //asignar ciertas carcateristicas de un tecnologico especifico
            };
            lb_ninguno.GestureRecognizers.Add(tapr);
            lb_seguir = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Seguir" ,
                TextColor = Color.Gray,
                FontSize = 18,
                FontFamily = "Roboto"
            };
            lb_seleccionar = new Label()
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Selecciona tu Institución",
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.Gray,
                FontSize = 26,
                FontFamily = "Roboto"
            };

            st_inst = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(20),
                Children =
                {
                    lb_seguir,
                    lb_seleccionar,
                    sr_inst,
                    ai_inst,
                    lv_inst,
                    bv_div,
                    lb_ninguno
                }
            };
            Content = st_inst;
        }

        private void buscarInstitucion(string institucion)
        {
            if(!string.IsNullOrWhiteSpace(institucion))
            {
                lv_inst.ItemsSource = list_inst.Where(x=>x.institution.ToLower().Contains(institucion.ToLower()));
            }
            else
            {
                lv_inst.ItemsSource = list_inst;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            lv_inst.IsVisible = false;
            ai_inst.IsRunning = true;
            list_inst = await objWsInst.listaInstituciones();
            lv_inst.ItemsSource = list_inst;
            ai_inst.IsRunning = false;
            ai_inst.IsVisible = false;
            lv_inst.IsVisible = true;

        }
        protected override bool OnBackButtonPressed()
        {
            DisplayAlert("UPS!", "selecciona una institución", "Aceptar");
            return true;
        }

    }
}
