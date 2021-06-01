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
            "MeshCentral Assistant",
            new Dictionary<string, string>() {
                {"ru","MeshCentral Ассистент"},
                {"fr","Assistant MeshCentral"},
                {"es","Asistente MeshCentral"}
            }
        },
        {
            "Loading...",
            new Dictionary<string, string>() {
                {"de","Laden..."},
                {"hi","लोड हो रहा है..."},
                {"fr","Chargement..."},
                {"zh-cht","載入中..."},
                {"zh-chs","载入中..."},
                {"fi","Ladataan..."},
                {"tr","Yükleniyor..."},
                {"cs","Načítání…"},
                {"ja","読み込み中..."},
                {"es","Cargando..."},
                {"pt","Carregando..."},
                {"nl","Laden..."},
                {"ko","불러오는 중 ..."},
                {"ru","Загрузка..."}
            }
        },
        {
            "User",
            new Dictionary<string, string>() {
                {"de","Benutzer"},
                {"hi","उपयोगकर्ता"},
                {"fr","Utilisateur"},
                {"zh-cht","用戶"},
                {"zh-chs","用户"},
                {"fi","Käyttäjä"},
                {"tr","Kullanıcı"},
                {"cs","Uživatel"},
                {"ja","ユーザー"},
                {"es","Usuario"},
                {"pt","Do utilizador"},
                {"nl","Gebruiker"},
                {"ko","사용자"},
                {"ru","Пользователь"}
            }
        },
        {
            "Notify",
            new Dictionary<string, string>() {
                {"de","Benachrichtigen"},
                {"hi","सूचित करें"},
                {"fr","Notifier"},
                {"zh-cht","通知"},
                {"zh-chs","通知"},
                {"fi","Ilmoita"},
                {"tr","Bildir"},
                {"cs","Upozornit"},
                {"ja","通知する"},
                {"es","Notificar"},
                {"pt","Notificar"},
                {"nl","Melden"},
                {"ko","알림"},
                {"ru","Уведомить"}
            }
        },
        {
            "Time",
            new Dictionary<string, string>() {
                {"de","Zeit"},
                {"hi","समय"},
                {"fr","Temps"},
                {"zh-cht","時間"},
                {"zh-chs","时间"},
                {"fi","Aika"},
                {"tr","Zaman"},
                {"cs","Čas"},
                {"ja","時間"},
                {"es","Tiempo"},
                {"pt","Tempo"},
                {"nl","Tijd"},
                {"ko","시간"},
                {"ru","Время"}
            }
        },
        {
            "Events",
            new Dictionary<string, string>() {
                {"de","Ereignisse"},
                {"hi","आयोजन"},
                {"fr","Événements"},
                {"zh-cht","事件"},
                {"zh-chs","事件"},
                {"fi","Tapahtumat"},
                {"tr","Etkinlikler"},
                {"cs","Události"},
                {"ja","イベント"},
                {"es","Eventos"},
                {"pt","Eventos"},
                {"nl","Gebeurtenissen"},
                {"ko","이벤트"},
                {"ru","События"}
            }
        },
        {
            "Type",
            new Dictionary<string, string>() {
                {"de","Typ"},
                {"hi","प्रकार"},
                {"zh-cht","類型"},
                {"zh-chs","类型"},
                {"fi","Tyyppi"},
                {"tr","tip"},
                {"cs","Typ"},
                {"ja","タイプ"},
                {"es","Tipo"},
                {"pt","Tipo"},
                {"ko","유형"},
                {"ru","Удаленный ввод"}
            }
        },
        {
            "Disconnected",
            new Dictionary<string, string>() {
                {"de","Getrennt"},
                {"hi","डिस्कनेक्ट किया गया"},
                {"fr","Débranché"},
                {"zh-cht","已斷線"},
                {"zh-chs","已断线"},
                {"fi","Yhteys katkaistu"},
                {"tr","Bağlantı kesildi"},
                {"cs","Odpojeno"},
                {"ja","切断されました"},
                {"es","Desconectado"},
                {"pt","Desconectado"},
                {"nl","Verbroken"},
                {"ko","연결 해제"},
                {"ru","Отключен"}
            }
        },
        {
            "State",
            new Dictionary<string, string>() {
                {"de","Zustand"},
                {"hi","राज्य"},
                {"fr","Etat"},
                {"zh-cht","狀態"},
                {"zh-chs","状况"},
                {"fi","Tila"},
                {"tr","Durum"},
                {"cs","Stav"},
                {"ja","状態"},
                {"es","Estado"},
                {"pt","Estado"},
                {"nl","Status"},
                {"ko","상태"},
                {"ru","Состояние"}
            }
        },
        {
            "Enabled",
            new Dictionary<string, string>() {
                {"de","aktiviert"},
                {"hi","सक्रिय"},
                {"fr","Activer"},
                {"zh-cht","已啟用"},
                {"zh-chs","已启用"},
                {"fi","Käytössä"},
                {"tr","Etkin"},
                {"cs","Povoleno"},
                {"ja","有効"},
                {"es","Habilitado"},
                {"pt","ativado"},
                {"nl","Ingeschakeld"},
                {"ko","활성화 됨"},
                {"ru","Включено"}
            }
        },
        {
            "Files",
            new Dictionary<string, string>() {
                {"de","Dateien"},
                {"hi","फ़ाइलें"},
                {"fr","Dossiers"},
                {"zh-cht","檔案"},
                {"zh-chs","档案"},
                {"fi","Tiedostot"},
                {"tr","Dosyalar"},
                {"cs","Soubory"},
                {"ja","ファイル"},
                {"es","Archivos"},
                {"pt","Arquivos"},
                {"nl","Bestanden"},
                {"ko","파일"},
                {"ru","Файлы"}
            }
        },
        {
            "Cancel",
            new Dictionary<string, string>() {
                {"de","Abbrechen"},
                {"hi","रद्द करना"},
                {"fr","Annuler"},
                {"zh-cht","取消"},
                {"zh-chs","取消"},
                {"fi","Peruuta"},
                {"tr","İptal etmek"},
                {"cs","Storno"},
                {"ja","キャンセル"},
                {"es","Cancelar"},
                {"pt","Cancelar"},
                {"nl","Annuleren"},
                {"ko","취소"},
                {"ru","Отмена"}
            }
        },
        {
            "Connected",
            new Dictionary<string, string>() {
                {"de","Verbunden"},
                {"hi","जुड़े हुए"},
                {"fr","Connecté"},
                {"zh-cht","已連接"},
                {"zh-chs","已连接"},
                {"fi","Yhdistetty"},
                {"tr","Bağlandı"},
                {"cs","Připojeno"},
                {"ja","接続済み"},
                {"es","Conectado"},
                {"pt","Conectado"},
                {"nl","Verbonden"},
                {"ko","연결됨"},
                {"ru","Подключено"}
            }
        },
        {
            "Close",
            new Dictionary<string, string>() {
                {"de","Schließen"},
                {"hi","बंद करे"},
                {"fr","Fermer"},
                {"zh-cht","關"},
                {"zh-chs","关"},
                {"fi","Sulje"},
                {"tr","Kapat"},
                {"cs","Zavřít"},
                {"ja","閉じる"},
                {"es","Cerrar"},
                {"pt","Fechar"},
                {"nl","Sluiten"},
                {"ko","닫기"},
                {"ru","Закрыть"}
            }
        },
        {
            "Disabled",
            new Dictionary<string, string>() {
                {"de","Deaktiviertes"},
                {"hi","विकलांग"},
                {"fr","Désactivé"},
                {"zh-cht","已禁用"},
                {"zh-chs","已禁用"},
                {"fi","Poistettu käytöstä"},
                {"tr","Devre dışı"},
                {"cs","Zakázáno"},
                {"ja","無効"},
                {"es","Deshabilitado"},
                {"pt","Desativado"},
                {"nl","Uitgeschakeld"},
                {"ko","비활성화"},
                {"ru","Отключено"}
            }
        },
        {
            "OK",
            new Dictionary<string, string>() {
                {"hi","ठीक"},
                {"fr","ОК"},
                {"tr","tamam"},
                {"pt","Ok"},
                {"ko","확인"},
                {"ru","ОК"}
            }
        },
        {
            "Not Activated (In)",
            new Dictionary<string, string>() {
                {"de","Nicht aktiviert (In)"},
                {"hi","सक्रिय नहीं (में)"},
                {"fr","Non activé (en)"},
                {"zh-cht","未啟動（輸入）"},
                {"zh-chs","未激活（输入）"},
                {"fi","Ei aktivoitu (sisään)"},
                {"tr","Etkinleştirilmedi (İçinde)"},
                {"cs","Neaktivováno (v)"},
                {"ja","アクティブ化されていない（イン）"},
                {"es","No Activada (entrada)"},
                {"pt","Não ativado (entrada)"},
                {"nl","Niet geactiveerd (In)"},
                {"ko","활성화되지 않음 (In)"},
                {"ru","Не активированно (In)"}
            }
        },
        {
            "Desktop",
            new Dictionary<string, string>() {
                {"hi","डेस्कटॉप"},
                {"fr","Bureau"},
                {"zh-cht","桌面"},
                {"zh-chs","桌面"},
                {"fi","Työpöytä"},
                {"tr","Masaüstü Bilgisayar"},
                {"cs","Plocha"},
                {"ja","デスクトップ"},
                {"es","Escritorio"},
                {"pt","Área de Trabalho"},
                {"nl","Bureaublad"},
                {"ko","데스크탑"},
                {"ru","Рабочий стол"}
            }
        },
        {
            "Unknown",
            new Dictionary<string, string>() {
                {"de","Unbekannt"},
                {"hi","अनजान"},
                {"fr","Inconnue"},
                {"zh-cht","未知"},
                {"zh-chs","未知"},
                {"fi","Tuntematon"},
                {"tr","Bilinmeyen"},
                {"cs","Neznámé"},
                {"ja","未知の"},
                {"es","Desconocido"},
                {"pt","Desconhecido"},
                {"nl","Onbekend"},
                {"ko","알 수 없는"},
                {"ru","Неизвестно"}
            }
        },
        {
            "Activated",
            new Dictionary<string, string>() {
                {"de","Aktiviertes"},
                {"hi","सक्रिय"},
                {"fr","Activé"},
                {"zh-cht","已啟動"},
                {"zh-chs","已激活"},
                {"fi","Aktivoitu"},
                {"tr","Aktif"},
                {"cs","Zapnuto"},
                {"ja","有効化"},
                {"es","Activado"},
                {"pt","ativado"},
                {"nl","Geactiveerd"},
                {"ko","활성화 됨"},
                {"ru","Активировано"}
            }
        },
        {
            "Terminal",
            new Dictionary<string, string>() {
                {"hi","टर्मिनल"},
                {"zh-cht","終端機"},
                {"zh-chs","终端"},
                {"fi","Pääte"},
                {"tr","terminal"},
                {"cs","Terminál"},
                {"ja","ターミナル"},
                {"ko","터미널"},
                {"ru","Терминал"}
            }
        },
        {
            "User Consent",
            new Dictionary<string, string>() {
                {"de","Benutzereinwilligung"},
                {"hi","उपयोगकर्ता सहमति"},
                {"fr","Consentement de l'utilisateur"},
                {"zh-cht","用戶同意"},
                {"zh-chs","用户同意"},
                {"fi","Käyttäjän suostumus"},
                {"tr","Kullanıcı Onayı"},
                {"cs","Souhlas uživatele"},
                {"ja","ユーザーの同意"},
                {"es","Consentimiento del Usuario"},
                {"pt","Consentimento do Usuário"},
                {"nl","Toestemming van gebruiker"},
                {"ko","사용자 연결 옵션"},
                {"ru","Согласие пользователя"}
            }
        },
        {
            "Not Activated (Pre)",
            new Dictionary<string, string>() {
                {"de","Nicht aktiviert (Pre)"},
                {"hi","सक्रिय नहीं (पूर्व)"},
                {"fr","Non activé (pré)"},
                {"zh-cht","未啟動（預）"},
                {"zh-chs","未激活（预）"},
                {"fi","Ei aktivoitu (ennakko)"},
                {"tr","Etkinleştirilmedi (Ön)"},
                {"cs","Neaktivováno (před)"},
                {"ja","アクティブ化されていない（前）"},
                {"es","No activada (Pre)"},
                {"pt","Não ativado (pré)"},
                {"nl","Niet geactiveerd (Pre)"},
                {"ko","활성화되지 않음 (Pre)"},
                {"ru","Не активированно (Pre)"}
            }
        },
        {
            "Send",
            new Dictionary<string, string>() {
                {"de","Senden"},
                {"hi","संदेश"},
                {"fr","Envoyer"},
                {"zh-cht","發送"},
                {"zh-chs","发送"},
                {"fi","Lähetä"},
                {"tr","Gönder"},
                {"cs","Odeslat"},
                {"ja","送る"},
                {"es","Enviar"},
                {"pt","Enviar"},
                {"nl","Verzenden"},
                {"ko","전송"},
                {"ru","Отправить"}
            }
        },
        {
            "Clear",
            new Dictionary<string, string>() {
                {"de","Leeren"},
                {"hi","स्पष्ट"},
                {"fr","Nettoyer"},
                {"zh-cht","清除"},
                {"zh-chs","清除"},
                {"fi","Tyhjennä"},
                {"tr","Açık"},
                {"cs","Vymazat"},
                {"ja","クリア"},
                {"es","Borrar"},
                {"pt","Limpo"},
                {"nl","Wissen"},
                {"ko","지우기"},
                {"ru","Очистить"}
            }
        },
        {
            "Privacy Bar",
            new Dictionary<string, string>() {
                {"de","Datenschutzleiste"},
                {"hi","गोपनीयता बार"},
                {"fr","Barre de confidentialité"},
                {"zh-cht","隱私欄"},
                {"zh-chs","隐私栏"},
                {"fi","Tietosuojapalkki"},
                {"tr","Gizlilik Çubuğu"},
                {"cs","Bar ochrany osobních údajů"},
                {"ja","プライバシーバー"},
                {"es","Barra de Privacidad"},
                {"pt","Barra de Privacidade"},
                {"nl","Privacy balk"},
                {"ko","프라이버시 바"},
                {"ru","Панель конфиденциальности"}
            }
        },
        {
            "Update",
            new Dictionary<string, string>() {
                {"de","Updates"},
                {"hi","अपडेट करें"},
                {"fr","Mettre à jour"},
                {"zh-cht","更新資料"},
                {"zh-chs","更新资料"},
                {"fi","Päivittää"},
                {"tr","Güncelleme"},
                {"cs","Aktualizace"},
                {"ja","更新"},
                {"es","Actualizar"},
                {"pt","Atualizar"},
                {"nl","Bijwerken"},
                {"ko","개조하다"},
                {"ru","Обновить"}
            }
        },
        {
            "Agent Console",
            new Dictionary<string, string>() {
                {"de","Agent-Konsole"},
                {"hi","एजेंट कंसोल"},
                {"fr","Console d'agent"},
                {"zh-cht","代理控制台"},
                {"zh-chs","代理控制台"},
                {"fi","Agentin konsoli"},
                {"tr","Aracı Konsolu"},
                {"cs","Konzole agenta"},
                {"ja","エージェントコンソール"},
                {"es","Consola de Agente"},
                {"pt","Console do agente"},
                {"nl","Agent console"},
                {"ko","에이전트 콘솔"},
                {"ru","Консоль агента"}
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
