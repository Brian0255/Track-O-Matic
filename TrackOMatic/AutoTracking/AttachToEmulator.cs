using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace TrackOMatic
{
    public static class AttachToEmulator
    {
        private static AttachedProcessInfo AttachToProject64(GameVerificationInfo verificationInfo, bool doOffsetScan = false)
        {
            Process target;
            try
            {
               target = Process.GetProcessesByName("project64")[0];
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.Message + "\nCould not find process \"project64\" on your machine.", "TrackOMatic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            uint romAddrStart = 0;

            /*for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        romAddrStart = 0xDFE40000;
                        break;
                    case 1:
                        romAddrStart = 0xDFE70000;
                        break;
                    case 2:
                        romAddrStart = 0xDFFB0000;
                        break;
                    default:
                        Debug.WriteLine("wasnt those 3 addresses");
                        return null;

                }

                int gamecheck = 0;
                try
                {
                    gamecheck = Memory.ReadInt32(target, romAddrStart + verificationInfo.TargetAddress);
                }
                catch (Exception e)
                {
                    return null;
                }

                if (gamecheck == verificationInfo.TargetValue)
                {
                    Debug.WriteLine("Verified Project64");
                    return new AttachedProcessInfo(target, romAddrStart);
                }
            }*/

            //Project64 versions can differ in memory so try different offsets
            for (uint potentialOffset = 0xDFD00000; potentialOffset < 0xE01FFFFF; potentialOffset += 16)
            {
                if (Memory.ReadInt32(target, potentialOffset + verificationInfo.TargetAddress) == verificationInfo.TargetValue)
                {
                    return new AttachedProcessInfo(target, potentialOffset);
                }
            }

            //System.Windows.Forms.MessageBox.Show("Could not find correct offset.", "TrackOMatic", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }


        private static Tuple<Process, uint> AttachToBizhawk()
        {
            /*
            Process target = null;
            try
            {
                target = Process.GetProcessesByName("emuhawk")[0];
            }
            catch (Exception)
            {
                return null;
            }
            Debug.WriteLine("found hawk");

            uint romAddrStart = 0;
            var gameInfo = getGameVerificationInfo(baseForm.CurrentLayout.App_Settings.AutotrackingGame);


            Int64 addressDLL = 0;
            foreach (ProcessModule mo in target.Modules)
            {
                if (mo.ModuleName.ToLower() == "mupen64plus.dll")
                {
                    addressDLL = mo.BaseAddress.ToInt64();
                    break;
                }
            }

            if (addressDLL == 0)
            {
                return null;
            }
            Debug.WriteLine("found dll");

            //Dim attemptOffset As Int64 = 0
            //for (int i = 0; i < 2; i++){
            //    switch (i)
            //    {
            //        case 0:
            //            romAddrStart = 0x658E0;
            //            break;
            //        case 1:
            //            romAddrStart = 0x658D0;
            //            break;
            //        default:
            //            return null;
            //    }

            //i'm too lazy to find common addresses, so i'm just gonna do a light bruteforce
            for (uint potOff = 0x5A000; potOff < 0x5658DF; potOff += 16)
            {
                romAddrStart = potOff;



                int gamecheck = 0;
                try
                {
                    if (gameInfo.Item2 == 8)
                    {
                        uint addr = Memory.Int8AddrFix(gameInfo.Item1);
                        gamecheck = Memory.ReadInt8(target, romAddrStart + addr);
                    }
                    else if (gameInfo.Item2 == 16)
                    {
                        uint addr = Memory.Int16AddrFix(gameInfo.Item1);
                        gamecheck = Memory.ReadInt16(target, romAddrStart + addr);
                    }
                    else if (gameInfo.Item2 == 32)
                    {
                        gamecheck = Memory.ReadInt32(target, (uint)(addressDLL + romAddrStart + gameInfo.Item1));
                    }
                    else
                    {
                        MessageBox.Show("Incorrect bytes set for verification.\nMust be either 8, 16, or 32", "GSTHD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("yeah bud shits fucked");
                    MessageBox.Show(e.Message, "GSTHD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                if (gamecheck == gameInfo.Item3)
                {

                    Debug.WriteLine("verifyably bizhawk");
                    return Tuple.Create(target, (uint)(addressDLL + romAddrStart));
                }
            }
            */
            return null;
        }


        private static Tuple<Process, ulong> AttachToRMG()
        {
            /*
            Process target = null;
            try
            {
                target = Process.GetProcessesByName("rmg")[0];
            }
            catch (Exception)
            {
                return null;
            }
            Debug.WriteLine("trans rights");

            var gameInfo = getGameVerificationInfo(baseForm.CurrentLayout.App_Settings.AutotrackingGame);


            ulong addressDLL = 0;
            foreach (ProcessModule mo in target.Modules)
            {
                if (mo.ModuleName.ToLower() == "mupen64plus.dll")
                {
                    addressDLL = (ulong)mo.BaseAddress.ToInt64();
                    break;
                }
            }

            if (addressDLL == 0)
            {
                return null;
            }
            Debug.WriteLine("found dll at 0x" + addressDLL.ToString("X"));

            for (uint potOff = 0x29C15D8; potOff < 0x2FC15D8; potOff += 16)
            {
                ulong romAddrStart = addressDLL + potOff;


                // read the address to find the address of the starting point in the rom
                ulong readAddress = Memory.ReadInt64(target, (romAddrStart));

                if (gameInfo.Item2 == 8)
                {
                    var addr = Memory.Int8AddrFix(readAddress + 0x80000000 + gameInfo.Item1);
                    var wherethefuck = Memory.ReadInt8(target, addr);
                    if ((wherethefuck & 0xff) == gameInfo.Item3)
                    {
                        return Tuple.Create(target, (readAddress + 0x80000000));

                    }
                }
                else if (gameInfo.Item2 == 16)
                {
                    var addr = Memory.Int16AddrFix(readAddress + 0x80000000 + gameInfo.Item1);
                    var wherethefuck = Memory.ReadInt16(target, addr);
                    if ((wherethefuck & 0xffff) == gameInfo.Item3)
                    {
                        return Tuple.Create(target, (readAddress + 0x80000000));

                    }
                }
                else if (gameInfo.Item2 == 32)
                {
                    // use this previously read address to find the game verification data
                    var wherethefuck = Memory.ReadInt32(target, (readAddress + 0x80000000 + gameInfo.Item1));
                    if ((wherethefuck & 0xffffffff) == gameInfo.Item3)
                    {
                        return Tuple.Create(target, (readAddress + 0x80000000));

                    }
                }
                else
                {
                    MessageBox.Show("Incorrect bytes set for verification.\nMust be either 8, 16, or 32", "GSTHD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }




            }
            */
            return null;
        }

        public static AttachedProcessInfo Attach(GameVerificationInfo verificationInfo, EmulatorName attachTo)
        {
            return AttachToProject64(verificationInfo, false);
        }


    }
}
