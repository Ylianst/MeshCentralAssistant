/*
Copyright 2009-2021 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace MeshAssistant
{
    public class Translate
    {
        // *** TRANSLATION TABLE START ***
        static private Dictionary<string, Dictionary<string, string>> translationTable = new Dictionary<string, Dictionary<string, string>>() {
        {
            "Authenticating",
            new Dictionary<string, string>() {
                {"tr","Authentification"},
                {"fr","Zfgsvmgrxzgrmt"}
            }
        },
        {
            "&Close",
            new Dictionary<string, string>() {
                {"tr","&Fermer"},
                {"fr","&Xolhv"}
            }
        },
        {
            "Deny",
            new Dictionary<string, string>() {
                {"tr","Refuser"},
                {"fr","Wvmb"}
            }
        },
        {
            "Agent is pause pending",
            new Dictionary<string, string>() {
                {"tr","L'agent est en attente de pause"},
                {"fr","Ztvmg rh kzfhv kvmwrmt"}
            }
        },
        {
            "State",
            new Dictionary<string, string>() {
                {"tr","État"},
                {"fr","Hgzgv"}
            }
        },
        {
            "Agent Select",
            new Dictionary<string, string>() {
                {"tr","Sélection d'agent"},
                {"fr","Ztvmg Hvovxg"}
            }
        },
        {
            "Help Requested",
            new Dictionary<string, string>() {
                {"tr","Aide demandée"},
                {"fr","Svok Ivjfvhgvw"}
            }
        },
        {
            "Cancel",
            new Dictionary<string, string>() {
                {"tr","Annuler"},
                {"fr","Xzmxvo"}
            }
        },
        {
            "Not Activated (In)",
            new Dictionary<string, string>() {
                {"tr","Non activé (Entrée)"},
                {"fr","Mlg Zxgrezgvw (Rm)"}
            }
        },
        {
            "Privacy Bar",
            new Dictionary<string, string>() {
                {"tr","Barre de confidentialité"},
                {"fr","Kirezxb Yzi"}
            }
        },
        {
            "S&top Agent",
            new Dictionary<string, string>() {
                {"tr","Agent d'arrêt"},
                {"fr","H&glk Ztvmg"}
            }
        },
        {
            "Agent not installed",
            new Dictionary<string, string>() {
                {"tr","Agent non installé"},
                {"fr","Ztvmg mlg rmhgzoovw"}
            }
        },
        {
            "User",
            new Dictionary<string, string>() {
                {"tr","Utilisateur"},
                {"fr","Fhvi"}
            }
        },
        {
            "Events",
            new Dictionary<string, string>() {
                {"tr","Événements"},
                {"fr","Vevmgh"}
            }
        },
        {
            "Agent is missing",
            new Dictionary<string, string>() {
                {"tr","L'agent est manquant"},
                {"fr","Ztvmg rh nrhhrmt"}
            }
        },
        {
            "{0} Assistant",
            new Dictionary<string, string>() {
                {"tr","{0} Assistante"},
                {"fr","{0} Zhhrhgzmg"}
            }
        },
        {
            "PrivacyBarForm",
            new Dictionary<string, string>() {
                {"fr","KirezxbYziUlin"}
            }
        },
        {
            "&Open",
            new Dictionary<string, string>() {
                {"tr","&Ouvert"},
                {"fr","&Lkvm"}
            }
        },
        {
            "Connected to server",
            new Dictionary<string, string>() {
                {"tr","Connecté au serveur"},
                {"fr","Xlmmvxgvw gl hvievi"}
            }
        },
        {
            "Later",
            new Dictionary<string, string>() {
                {"tr","Plus tard"},
                {"fr","Ozgvi"}
            }
        },
        {
            "Close",
            new Dictionary<string, string>() {
                {"tr","Fermer"},
                {"fr","Xolhv"}
            }
        },
        {
            "Agent is stopped",
            new Dictionary<string, string>() {
                {"tr","L'agent est arrêté"},
                {"fr","Ztvmg rh hglkkvw"}
            }
        },
        {
            "Agent Snapshot",
            new Dictionary<string, string>() {
                {"tr","Instantané de l'agent"},
                {"fr","Ztvmg Hmzkhslg"}
            }
        },
        {
            "{0} remote sessions",
            new Dictionary<string, string>() {
                {"tr","{0} sessions à distance"},
                {"fr","{0} ivnlgv hvhhrlmh"}
            }
        },
        {
            "Cancel Help Request",
            new Dictionary<string, string>() {
                {"tr","Annuler la demande d'aide"},
                {"fr","Xzmxvo Svok Ivjfvhg"}
            }
        },
        {
            "TCP relay",
            new Dictionary<string, string>() {
                {"tr","Relais TCP"},
                {"fr","GXK ivozb"}
            }
        },
        {
            "Clear",
            new Dictionary<string, string>() {
                {"tr","Dégager"},
                {"fr","Xovzi"}
            }
        },
        {
            "(None)",
            new Dictionary<string, string>() {
                {"tr","(Rien)"},
                {"fr","(Mlmv)"}
            }
        },
        {
            "Count",
            new Dictionary<string, string>() {
                {"tr","Compter"},
                {"fr","Xlfmg"}
            }
        },
        {
            "Value",
            new Dictionary<string, string>() {
                {"tr","Valeur"},
                {"fr","Ezofv"}
            }
        },
        {
            "Direct Connect",
            new Dictionary<string, string>() {
                {"tr","Connection directe"},
                {"fr","Wrivxg Xlmmvxg"}
            }
        },
        {
            "Remote Sessions: {0}",
            new Dictionary<string, string>() {
                {"tr","Sessions à distance : {0}"},
                {"fr","Ivnlgv Hvhhrlmh: {0}"}
            }
        },
        {
            "Agent is disconnected",
            new Dictionary<string, string>() {
                {"tr","L'agent est déconnecté"},
                {"fr","Ztvmg rh wrhxlmmvxgvw"}
            }
        },
        {
            "Request Help",
            new Dictionary<string, string>() {
                {"tr","Demander de l'aide"},
                {"fr","Ivjfvhg Svok"}
            }
        },
        {
            "Intel® ME State...",
            new Dictionary<string, string>() {
                {"tr","État Intel® ME..."},
                {"fr","Rmgvo® NV Hgzgv..."}
            }
        },
        {
            "List of remote sessions active on this computer.",
            new Dictionary<string, string>() {
                {"tr","Liste des sessions distantes actives sur cet ordinateur."},
                {"fr","Orhg lu ivnlgv hvhhrlmh zxgrev lm gsrh xlnkfgvi."}
            }
        },
        {
            "Not Activated (Pre)",
            new Dictionary<string, string>() {
                {"tr","Non activé (pré)"},
                {"fr","Mlg Zxgrezgvw (Kiv)"}
            }
        },
        {
            "A new version of this software is available. Update now?",
            new Dictionary<string, string>() {
                {"tr","Une nouvelle version de ce logiciel est disponible. Mettez à jour maintenant?"},
                {"fr","Z mvd evihrlm lu gsrh hlugdziv rh zezrozyov. Fkwzgv mld?"}
            }
        },
        {
            "{0} - {1}",
            new Dictionary<string, string>() {

            }
        },
        {
            "User Consent",
            new Dictionary<string, string>() {
                {"tr","Consentement de l'utilisateur"},
                {"fr","Fhvi Xlmhvmg"}
            }
        },
        {
            "&Update Software",
            new Dictionary<string, string>() {
                {"tr","&Mettre à jour le logiciel"},
                {"fr","&Fkwzgv Hlugdziv"}
            }
        },
        {
            "Remote Sessions...",
            new Dictionary<string, string>() {
                {"tr","Séances à distance..."},
                {"fr","Ivnlgv Hvhhrlmh..."}
            }
        },
        {
            "Intel® Management Engine",
            new Dictionary<string, string>() {
                {"tr","Moteur de gestion Intel®"},
                {"fr","Rmgvo® Nzmztvnvmg Vmtrmv"}
            }
        },
        {
            "Send",
            new Dictionary<string, string>() {
                {"tr","Envoyer"},
                {"fr","Hvmw"}
            }
        },
        {
            "Enter help request details",
            new Dictionary<string, string>() {
                {"tr","Entrez les détails de la demande d'aide"},
                {"fr","Vmgvi svok ivjfvhg wvgzroh"}
            }
        },
        {
            "Terminal",
            new Dictionary<string, string>() {
                {"fr","Gvinrmzo"}
            }
        },
        {
            "Request Help...",
            new Dictionary<string, string>() {
                {"tr","Demander de l'aide..."},
                {"fr","Ivjfvhg Svok..."}
            }
        },
        {
            "Agent is stopped pending",
            new Dictionary<string, string>() {
                {"tr","L'agent est arrêté en attente"},
                {"fr","Ztvmg rh hglkkvw kvmwrmt"}
            }
        },
        {
            "Disconnected",
            new Dictionary<string, string>() {
                {"tr","Débranché"},
                {"fr","Wrhxlmmvxgvw"}
            }
        },
        {
            "Remote Sessions",
            new Dictionary<string, string>() {
                {"tr","Séances à distance"},
                {"fr","Ivnlgv Hvhhrlmh"}
            }
        },
        {
            "Type",
            new Dictionary<string, string>() {
                {"tr","Taper"},
                {"fr","Gbkv"}
            }
        },
        {
            "1 remote session",
            new Dictionary<string, string>() {
                {"tr","1 séance à distance"},
                {"fr","1 ivnlgv hvhhrlm"}
            }
        },
        {
            "Assistant Update",
            new Dictionary<string, string>() {
                {"tr","Mise à jour de l'assistant"},
                {"fr","Zhhrhgzmg Fkwzgv"}
            }
        },
        {
            "Connecting",
            new Dictionary<string, string>() {
                {"tr","De liaison"},
                {"fr","Xlmmvxgrmt"}
            }
        },
        {
            "Agent is start pending",
            new Dictionary<string, string>() {
                {"tr","L'agent est en attente de démarrage"},
                {"fr","Ztvmg rh hgzig kvmwrmt"}
            }
        },
        {
            "Loading...",
            new Dictionary<string, string>() {
                {"tr","Chargement..."},
                {"fr","Olzwrmt..."}
            }
        },
        {
            "E&xit",
            new Dictionary<string, string>() {
                {"tr","Sortir"},
                {"fr","V&crg"}
            }
        },
        {
            "Show Sessions...",
            new Dictionary<string, string>() {
                {"tr","Afficher les séances..."},
                {"fr","Hsld Hvhhrlmh..."}
            }
        },
        {
            "{0} remote sessions are active.",
            new Dictionary<string, string>() {
                {"tr","{0} sessions à distance sont actives."},
                {"fr","{0} ivnlgv hvhhrlmh ziv zxgrev."}
            }
        },
        {
            "No remote sessions",
            new Dictionary<string, string>() {
                {"tr","Pas de sessions à distance"},
                {"fr","Ml ivnlgv hvhhrlmh"}
            }
        },
        {
            "Agent is continue pending",
            new Dictionary<string, string>() {
                {"tr","L'agent est en attente de poursuite"},
                {"fr","Ztvmg rh xlmgrmfv kvmwrmt"}
            }
        },
        {
            "Allow",
            new Dictionary<string, string>() {
                {"tr","Permettre"},
                {"fr","Zoold"}
            }
        },
        {
            "1 remote session is active.",
            new Dictionary<string, string>() {
                {"tr","1 session à distance est active."},
                {"fr","1 ivnlgv hvhhrlm rh zxgrev."}
            }
        },
        {
            "Notify",
            new Dictionary<string, string>() {
                {"tr","Avertir"},
                {"fr","Mlgrub"}
            }
        },
        {
            "OK",
            new Dictionary<string, string>() {
                {"tr","d'accord"},
                {"fr","LP"}
            }
        },
        {
            "Agent Console",
            new Dictionary<string, string>() {
                {"tr","Console des agents"},
                {"fr","Ztvmg Xlmhlov"}
            }
        },
        {
            "Files",
            new Dictionary<string, string>() {
                {"tr","Des dossiers"},
                {"fr","Urovh"}
            }
        },
        {
            "Agent is running",
            new Dictionary<string, string>() {
                {"tr","L'agent est en cours d'exécution"},
                {"fr","Ztvmg rh ifmmrmt"}
            }
        },
        {
            "O&pen Site...",
            new Dictionary<string, string>() {
                {"tr","&Ouvrir le site..."},
                {"fr","L&kvm Hrgv..."}
            }
        },
        {
            "Time",
            new Dictionary<string, string>() {
                {"tr","Temps"},
                {"fr","Grnv"}
            }
        },
        {
            "MeshCentral Assistant",
            new Dictionary<string, string>() {
                {"tr","Assistant MeshCentral"},
                {"fr","NvhsXvmgizo Zhhrhgzmg"}
            }
        },
        {
            "Connected",
            new Dictionary<string, string>() {
                {"tr","Lié"},
                {"fr","Xlmmvxgvw"}
            }
        },
        {
            "Multiple Users",
            new Dictionary<string, string>() {
                {"tr","Utilisateurs multiples"},
                {"fr","Nfogrkov Fhvih"}
            }
        },
        {
            "Item",
            new Dictionary<string, string>() {
                {"tr","Article"},
                {"fr","Rgvn"}
            }
        },
        {
            "Enabled",
            new Dictionary<string, string>() {
                {"tr","Activée"},
                {"fr","Vmzyovw"}
            }
        },
        {
            "Event",
            new Dictionary<string, string>() {
                {"tr","Événement"},
                {"fr","Vevmg"}
            }
        },
        {
            "Activated",
            new Dictionary<string, string>() {
                {"tr","Activé"},
                {"fr","Zxgrezgvw"}
            }
        },
        {
            "Show &Events...",
            new Dictionary<string, string>() {
                {"tr","Afficher les &événements..."},
                {"fr","Hsld &Vevmgh..."}
            }
        },
        {
            "No active remote sessions.",
            new Dictionary<string, string>() {
                {"tr","Aucune session à distance active."},
                {"fr","Ml zxgrev ivnlgv hvhhrlmh."}
            }
        },
        {
            "Desktop",
            new Dictionary<string, string>() {
                {"tr","Bureau"},
                {"fr","Wvhpglk"}
            }
        },
        {
            "Versions",
            new Dictionary<string, string>() {
                {"fr","Evihrlmh"}
            }
        },
        {
            "Intel® Management Engine state for this computer.",
            new Dictionary<string, string>() {
                {"tr","État du moteur de gestion Intel® pour cet ordinateur."},
                {"fr","Rmgvo® Nzmztvnvmg Vmtrmv hgzgv uli gsrh xlnkfgvi."}
            }
        },
        {
            "Unknown",
            new Dictionary<string, string>() {
                {"tr","Inconnu"},
                {"fr","Fmpmldm"}
            }
        },
        {
            "Disabled",
            new Dictionary<string, string>() {
                {"tr","Désactivée"},
                {"fr","Wrhzyovw"}
            }
        },
        {
            "UDP relay",
            new Dictionary<string, string>() {
                {"tr","relais UDP"},
                {"fr","FWK ivozb"}
            }
        },
        {
            "&Start Agent",
            new Dictionary<string, string>() {
                {"tr","&Démarrer l'agent"},
                {"fr","&Hgzig Ztvmg"}
            }
        },
        {
            "Agent is paused",
            new Dictionary<string, string>() {
                {"tr","L'agent est en pause"},
                {"fr","Ztvmg rh kzfhvw"}
            }
        },
        {
            "Update",
            new Dictionary<string, string>() {
                {"tr","Mettre à jour"},
                {"fr","Fkwzgv"}
            }
        }
        };
        // *** TRANSLATION TABLE END ***

        static public string T(string english)
        {
            string lang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if (lang == "en") return english;
            if (translationTable.ContainsKey(english))
            {
                Dictionary<string, string> translations = translationTable[english];
                if (translations.ContainsKey(lang)) return translations[lang];
            }
            return english;
        }

        static public void TranslateControl(Control control)
        {
            control.Text = T(control.Text);
            foreach (Control c in control.Controls) { TranslateControl(c); }
        }

        static public void TranslateContextMenu(ContextMenuStrip menu)
        {
            menu.Text = T(menu.Text);
            foreach (object i in menu.Items) { if (i.GetType() == typeof(ToolStripMenuItem)) { TranslateToolStripMenuItem((ToolStripMenuItem)i); } }
        }

        static public void TranslateToolStripMenuItem(ToolStripMenuItem menu)
        {
            menu.Text = T(menu.Text);
            foreach (object i in menu.DropDownItems)
            {
                if (i.GetType() == typeof(ToolStripMenuItem))
                {
                    TranslateToolStripMenuItem((ToolStripMenuItem)i);
                }
            }
        }

        static public void TranslateListView(ListView listview)
        {
            listview.Text = T(listview.Text);
            foreach (object c in listview.Columns)
            {
                if (c.GetType() == typeof(ColumnHeader))
                {
                    ((ColumnHeader)c).Text = T(((ColumnHeader)c).Text);
                }
            }
        }


    }
}
