using Magic.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Magic.Helpers
{

    public class TreatmentHelper
    {
        private readonly MagicEntities _entities = new MagicEntities();
        private string _path;
        private Edition _edition;

        public bool TreatAllNoneTreat()
        {
            try
            {
                var cardsNonTreated = _entities.Cards.Where(c => c.IsTreated == false).ToList();
                TreatCardRecursive(cardsNonTreated);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool SetAllAsNotTreated()
        {
            try
            {
                var cards = _entities.Cards.ToList();
                cards.ForEach(c => c.IsTreated = false);

                if (Directory.Exists(_path))
                {
                    Directory.Delete(_path, true);
                }

                _entities.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool TreatEdition(int editionId)
        {
            try
            {
                _edition = _entities.Editions.Single(e => e.Id == editionId);

                var cardsNonTreated = _edition.Cards.Where(c => c.IsTreated == false).Take(10).ToList();

                TreatCardRecursive(cardsNonTreated);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool TreatCard(string codeName)
        {

            try
            {
                var card = _entities.Cards.Single(c => c.CodeName == codeName);

                return Treat(card);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void TreatCardRecursive(List<Card> cards)
        {
            foreach (var cardNonTreated in cards)
            {
                Treat(cardNonTreated);
            }


            //Je sais plus a quoi ca sert mais un jour ca peut servir :D
            //var cardsNonTreated = _edition.Cards.Where(c => c.IsTreated == false)?.Take(10)?.ToList();

            //if (cardsNonTreated.Count > 0)
            //{
            //    TreatCardRecursive(cardsNonTreated);
            //}

        }

        private bool Treat(Card card)
        {
            if (!card.IsTreated)
            {
                ManageDirectory(card);
                DownloadAndCrop(card);
                _entities.SaveChanges();
                return true;
            }
            return false;
        }

        private void ManageDirectory(Card card)
        {
            var imagesDirectory = ConfigurationManager.AppSettings.GetValues("imagesDirectory");

            _path = imagesDirectory != null ? imagesDirectory[0] + "\\" + card.Edition.Title : "";

            if (!Directory.Exists(_path))
            {
                var securityDirectory = Directory.CreateDirectory(_path).GetAccessControl();
                securityDirectory.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    FileSystemRights.FullControl | FileSystemRights.Synchronize,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.InheritOnly, AccessControlType.Allow));

                Directory.SetAccessControl(_path, securityDirectory);

            }
        }

        private void DownloadAndCrop(Card card)
        {
            try
            {
                string cardPath;
                using (WebClient client = new WebClient())
                {
                    cardPath = _path + "\\" + card.CodeName + ".jpg";
                    client.DownloadFile(new Uri(card.UrlImage), cardPath);
                }
                Bitmap croppedImage = null;

                using (var originalImage = new Bitmap(cardPath))
                {
                    if (originalImage.Width == 312 && originalImage.Height == 445)
                    {
                        Rectangle crop = new Rectangle(20, 44, 275, 206);

                        croppedImage = originalImage.Clone(crop, originalImage.PixelFormat);
                    }
                }

                if (croppedImage != null)
                {
                    croppedImage.Save(cardPath, ImageFormat.Jpeg);
                    croppedImage.Dispose();
                    card.IsTreated = true;
                }
            }
            catch (Exception e) { throw new Exception(e.Message); }

        }
    }
}