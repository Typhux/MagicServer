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

        public List<Card> TreatEdition(int editionId)
        {
            _edition = _entities.Editions.Single(e => e.Id == editionId);

            var cardsNonTreated = _edition.Cards.Where(c => c.IsTreated == false).Take(10).ToList();

            TreatCardRecursive(cardsNonTreated);

            return _edition.Cards.ToList();
        }

        public Card TreatCard(string codeName)
        {
            var card = _entities.Cards.Single(c => c.CodeName == codeName);

            return Treat(card);
        }

        private void TreatCardRecursive(List<Card> cards)
        {
            foreach (var cardNonTreated in cards)
            {
                Treat(cardNonTreated);
            }

            var cardsNonTreated = _edition.Cards.Where(c => c.IsTreated == false).Take(10).ToList();

            if (cardsNonTreated.Count > 0)
            {
                TreatCardRecursive(cardsNonTreated);
            }

        }

        private Card Treat(Card card)
        {
            if (!card.IsTreated)
            {
                ManageDirectory(card);
                DownloadAndCrop(card);

                card.IsTreated = true;
                _entities.SaveChanges();
                return card;
            }
            return card;
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
                Bitmap croppedImage;

                using (var originalImage = new Bitmap(cardPath))
                {
                    Rectangle crop = new Rectangle(20, 44, 275, 206);

                    croppedImage = originalImage.Clone(crop, originalImage.PixelFormat);
                }

                croppedImage.Save(cardPath, ImageFormat.Jpeg);
                croppedImage.Dispose();
            }
            catch (Exception e) { throw new Exception(e.Message); }

        }
    }
}