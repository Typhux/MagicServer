using Magic.Helpers;
using Magic.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace Magic.Controllers.API
{
    public class EmulatorController : ApiController
    {

        private readonly EmulatorHelper emulatorHelper = new EmulatorHelper();

        [Route("api/emulator")]
        [HttpGet]
        public List<ResponseEmulator> Get()
        {
            return emulatorHelper.GetEmulators();
        }

        [Route("api/emulator/new/{id}/{codeName}")]
        [HttpGet]
        public ResponseEmulator New(int id, string codeName)
        {
            return emulatorHelper.NewEmulator(id, codeName);
        }

        [Route("api/emulator/reset/{id}")]
        [HttpGet]
        public ResponseEmulator Reset(int id)
        {
            return emulatorHelper.Reset(id);
        }

        [Route("api/emulator/endfight/{id}")]
        [HttpGet]
        public ResponseEmulator EndFight(int id)
        {
            return emulatorHelper.EndFight(id);
        }

        [Route("api/emulator/attack/{id}")]
        [HttpGet]
        public ResponseEmulator Attack(int id)
        {
            return emulatorHelper.Attack(id);
        }

        [Route("api/emulator/playspell/{id}/{guid}")]
        [HttpGet]
        public ResponseEmulator PlaySpell(int id, string guid)
        {
            return emulatorHelper.CallPlaySpell(id, guid);
        }

        [Route("api/emulator/turn/{id}")]
        [HttpGet]
        public ResponseEmulator Turn(int id)
        {
            return emulatorHelper.Turn(id);
        }

        #region ADD

        [Route("api/emulator/addenemy/{id}/{codeName}")]
        [HttpGet]
        public ResponseEmulator AddEnemy(int id, string codeName)
        {
            return emulatorHelper.AddEnemy(id, codeName);
        }

        [Route("api/emulator/addally/{id}/{codeName}")]
        [HttpGet]
        public ResponseEmulator AddAlly(int id, string codeName)
        {
            return emulatorHelper.AddAlly(id, codeName);
        }

        [Route("api/emulator/addartefact/{id}/{codeName}")]
        [HttpGet]
        public ResponseEmulator AddArtefact(int id, string codeName)
        {
            return emulatorHelper.AddArtefact(id, codeName);
        }

        [Route("api/emulator/addspell/{id}/{codeName}")]
        [HttpGet]
        public ResponseEmulator AddSpell(int id, string codeName)
        {
            return emulatorHelper.AddSpell(id, codeName);
        }

        #endregion

        [Route("api/emulator/{id}")]
        [HttpGet]
        public ResponseEmulator Get(int id)
        {
            return emulatorHelper.GetResponseEmulator(id);
        }

        [Route("api/emulator/changevalue/{id}")]
        [HttpPost]
        public ResponseEmulator ChangeValue(int id, [FromBody] SettingsEmulator settings)
        {
            return emulatorHelper.ChangeValue(id,settings);
        }

        [Route("api/emulator/getcodenamecard")]
        [HttpGet]
        public List<string> GetCodeNameCard()
        {
            return emulatorHelper.GetCodeNameCard();
        }

        [Route("api/emulator/treatpanel/{id}")]
        [HttpPost]
        public ResponseEmulator TreatPanel(int id, [FromBody] ActionPanel actionPanel)
        {
            return emulatorHelper.TreatPanel(id, actionPanel);
        }

        [Route("api/emulator/{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
            emulatorHelper.DeleteEmulator(id);
        }

        [Route("api/emulator/vampire/{id}/{guid}")]
        [HttpGet]
        public ResponseEmulator Vampire(int id, string guid)
        {
            return emulatorHelper.Vampire(id, guid);
        }

        [Route("api/emulator/equipartifact/{id}/{codeName}/{slot}")]
        [HttpGet]
        public ResponseEmulator EquipArtifact(int id, string codeName, int slot)
        {
            return emulatorHelper.EquipArtifact(id, codeName, slot);
        }

        [Route("api/emulator/unequipartifact/{id}/{slot}")]
        [HttpGet]
        public ResponseEmulator UnEquipArtifact(int id, int slot)
        {
            return emulatorHelper.UnEquipArtifact(id, slot);
        }
    }
}
