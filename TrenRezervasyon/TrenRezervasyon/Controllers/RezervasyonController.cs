using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrenRezervasyon.Models;
using System.Linq;


namespace TrenRezervasyon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RezervasyonController : ControllerBase
    {
        [HttpPost("Rezervasyon")]
        public RezervasyonSonucu Get([FromBody] Rezervasyon rez)
        {
            var rezervasyonIslemi = rez;
            List<Vagonlar>? KapasiteliVagonlar = new List<Vagonlar>();

            RezervasyonSonucu rezSonucu = new RezervasyonSonucu();
            int toplamKoltuk = 0;

            foreach (var vagon in rezervasyonIslemi.Tren.Vagonlar)
            {
               
                if (vagon.DoluKoltukAdet < Convert.ToDouble(vagon.Kapasite*0.7))
                {
                    toplamKoltuk += Convert.ToInt32(Convert.ToDouble(vagon.Kapasite * 0.7) - vagon.DoluKoltukAdet);
                    KapasiteliVagonlar.Add(vagon);
                }
                
            }

            if (KapasiteliVagonlar.Count> 0)
            {
                if (rezervasyonIslemi.KisilerFarkliVagonlaraYerlestirilebilir == false)
                {
                    int kisiSayisi = rezervasyonIslemi.RezervasyonYapilacakKisiSayisi;
           
                    YerlesimAyrinti trenVeYolcuBilgisi = new YerlesimAyrinti();

                    foreach (var vagon in KapasiteliVagonlar)
                    {
                        int bosKoltuk = Convert.ToInt32(Convert.ToDouble(vagon.Kapasite * 0.7) - vagon.DoluKoltukAdet);
                        if (bosKoltuk > kisiSayisi)
                        {
                            rezSonucu.RezervasyonYapilabilir = true;
                            trenVeYolcuBilgisi.VagonAdi = vagon.Ad;
                            trenVeYolcuBilgisi.KisiSayisi = rezervasyonIslemi.RezervasyonYapilacakKisiSayisi;
                            rezSonucu.YerlesimAyrinti = new List<YerlesimAyrinti>();
                            rezSonucu.YerlesimAyrinti.Add(trenVeYolcuBilgisi);
                        }
                        else
                            rezSonucu.RezervasyonYapilabilir = false;
                    }
                }

                else
                {
                    
                    int kisiSayisi = rezervasyonIslemi.RezervasyonYapilacakKisiSayisi;
                    if (toplamKoltuk > kisiSayisi)
                    {
                        rezSonucu.YerlesimAyrinti = new List<YerlesimAyrinti>();
                        foreach (var vagon in KapasiteliVagonlar)
                        {
                            YerlesimAyrinti trenVeYolcuBilgisi = new YerlesimAyrinti();
                            int bosKoltuk = Convert.ToInt32(Convert.ToDouble(vagon.Kapasite * 0.7) - vagon.DoluKoltukAdet);
                            if (kisiSayisi >= 0)
                            {

                                if (bosKoltuk > 0)
                                {
                                    if (kisiSayisi > bosKoltuk)
                                    {
                                        rezSonucu.RezervasyonYapilabilir = true;

                                        trenVeYolcuBilgisi.VagonAdi = vagon.Ad;
                                        trenVeYolcuBilgisi.KisiSayisi = bosKoltuk;

                                        rezSonucu.YerlesimAyrinti.Add(trenVeYolcuBilgisi);
                                        kisiSayisi -= bosKoltuk;
                                        continue;
                                    }
                                    else
                                    {
                                        rezSonucu.RezervasyonYapilabilir = true;

                                        trenVeYolcuBilgisi.VagonAdi = vagon.Ad;
                                        trenVeYolcuBilgisi.KisiSayisi = kisiSayisi;
                                        rezSonucu.YerlesimAyrinti.Add(trenVeYolcuBilgisi);

                                    }
                                }
                            }
                            else
                                rezSonucu.RezervasyonYapilabilir = false;
                        }
                    }
                 }
                return rezSonucu;
            }
            else
            {
                rezSonucu.RezervasyonYapilabilir = false;
            }
            
            return rezSonucu;
         
        }
    }
}
