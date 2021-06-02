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
            "Count",
            new Dictionary<string, string>() {
                {"ko","カウント"},
                {"fr","Compter"},
                {"es","Contar"},
                {"ja","カウント"},
                {"hi","गिनती"},
                {"zh-chs","数数"},
                {"de","Anzahl"}
            }
        },
        {
            "Intel® ME State...",
            new Dictionary<string, string>() {
                {"ko","インテル® ME 状態..."},
                {"fr","État Intel® ME..."},
                {"es","Estado Intel® ME ..."},
                {"ja","インテル® ME 状態..."},
                {"hi","इंटेल® एमई स्टेट..."},
                {"zh-chs","英特尔® ME 状态..."},
                {"de","Intel® ME-Zustand..."}
            }
        },
        {
            "Remote Sessions...",
            new Dictionary<string, string>() {
                {"ko","リモート セッション..."},
                {"fr","Séances à distance..."},
                {"es","Sesiones remotas ..."},
                {"ja","リモート セッション..."},
                {"hi","दूरस्थ सत्र..."},
                {"zh-chs","远程会话..."},
                {"de","Remote-Sitzungen..."}
            }
        },
        {
            "Remote Sessions",
            new Dictionary<string, string>() {
                {"ko","リモート セッション"},
                {"fr","Séances à distance"},
                {"es","Sesiones remotas"},
                {"ja","リモート セッション"},
                {"hi","दूरस्थ सत्र"},
                {"zh-chs","远程会话"},
                {"de","Remote-Sitzungen"}
            }
        },
        {
            "MeshCentral Assistant",
            new Dictionary<string, string>() {
                {"ko","MeshCentralアシスタント"},
                {"fr","Assistant MeshCentral"},
                {"zh-chs","MeshCentral 助手"},
                {"es","Asistente MeshCentral"},
                {"ja","MeshCentralアシスタント"},
                {"ru","MeshCentral Ассистент"},
                {"hi","मेषकेंद्रीय सहायक"},
                {"de","MeshCentral-Assistent"}
            }
        },
        {
            "&Start Agent",
            new Dictionary<string, string>() {
                {"ko","エージェントを開始"},
                {"fr","&Démarrer l'agent"},
                {"es","&Iniciar agente"},
                {"ja","エージェントを開始"},
                {"hi","&प्रारंभ एजेंट"},
                {"zh-chs","启动代理 (&S)"},
                {"de","&Agent starten"}
            }
        },
        {
            "Agent Select",
            new Dictionary<string, string>() {
                {"ko","エージェントセレクト"},
                {"fr","Sélection d'agent"},
                {"es","Seleccionar agente"},
                {"ja","エージェントセレクト"},
                {"hi","एजेंट चुनें"},
                {"zh-chs","代理选择"},
                {"de","Agentenauswahl"}
            }
        },
        {
            "Agent is paused",
            new Dictionary<string, string>() {
                {"ko","エージェントは一時停止しています"},
                {"fr","L'agent est en pause"},
                {"es","El agente está en pausa"},
                {"ja","エージェントは一時停止しています"},
                {"hi","एजेंट रुका हुआ है"},
                {"zh-chs","代理已暂停"},
                {"de","Agent ist pausiert"}
            }
        },
        {
            "Agent is continue pending",
            new Dictionary<string, string>() {
                {"ko","エージェントは保留中です"},
                {"fr","L'agent est en attente de poursuite"},
                {"es","El agente sigue pendiente"},
                {"ja","エージェントは保留中です"},
                {"hi","एजेंट जारी है लंबित"},
                {"zh-chs","代理正在继续等待"},
                {"de","Agent ist weiterhin ausstehend"}
            }
        },
        {
            "Later",
            new Dictionary<string, string>() {
                {"ko","後で"},
                {"fr","Plus tard"},
                {"es","Mas tarde"},
                {"ja","後で"},
                {"hi","बाद में"},
                {"zh-chs","之后"},
                {"de","Später"}
            }
        },
        {
            "Agent is stopped pending",
            new Dictionary<string, string>() {
                {"ko","エージェントが停止中です"},
                {"fr","L'agent est arrêté en attente"},
                {"es","El agente está detenido pendiente"},
                {"ja","エージェントが停止中です"},
                {"hi","एजेंट लंबित है"},
                {"zh-chs","代理停止等待"},
                {"de","Agent ist gestoppt ausstehend"}
            }
        },
        {
            "Agent is missing",
            new Dictionary<string, string>() {
                {"ko","エージェントが行方不明"},
                {"fr","L'agent est manquant"},
                {"es","Falta el agente"},
                {"ja","エージェントが行方不明"},
                {"hi","एजेंट गायब है"},
                {"zh-chs","代理不见了"},
                {"de","Agent fehlt"}
            }
        },
        {
            "Intel® Management Engine state for this computer.",
            new Dictionary<string, string>() {
                {"ko","このコンピューターの Intel® Management Engine の状態。"},
                {"fr","État du moteur de gestion Intel® pour cet ordinateur."},
                {"es","Estado del motor de administración Intel® para este equipo."},
                {"ja","このコンピューターの Intel® Management Engine の状態。"},
                {"hi","इस कंप्यूटर के लिए Intel® प्रबंधन इंजन स्थिति।"},
                {"zh-chs","此计算机的英特尔® 管理引擎状态。"},
                {"de","Status der Intel® Management Engine für diesen Computer."}
            }
        },
        {
            "Item",
            new Dictionary<string, string>() {
                {"ko","項目"},
                {"fr","Article"},
                {"es","Artículo"},
                {"ja","項目"},
                {"hi","मद"},
                {"zh-chs","物品"},
                {"de","Artikel"}
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
            "&Update Software",
            new Dictionary<string, string>() {
                {"ko","&アップデートソフトウェア"},
                {"fr","&Mettre à jour le logiciel"},
                {"es","&Actualiza el software"},
                {"ja","&アップデートソフトウェア"},
                {"hi","&सॉफ्टवेयर अद्यतन करें"},
                {"zh-chs","更新软件 (&S)"},
                {"de","&Software aktualisieren"}
            }
        },
        {
            "Agent is disconnected",
            new Dictionary<string, string>() {
                {"ko","エージェントが切断されました"},
                {"fr","L'agent est déconnecté"},
                {"es","El agente está desconectado"},
                {"ja","エージェントが切断されました"},
                {"hi","एजेंट डिस्कनेक्ट हो गया है"},
                {"zh-chs","代理已断开连接"},
                {"de","Agent ist getrennt"}
            }
        },
        {
            "Request Help",
            new Dictionary<string, string>() {
                {"ko","助けを求める"},
                {"fr","Demander de l'aide"},
                {"es","Solicitar ayuda"},
                {"ja","助けを求める"},
                {"hi","मदद का अनुरोध करें"},
                {"zh-chs","请求帮助"},
                {"de","Hilfe anfordern"}
            }
        },
        {
            "Remote Sessions: {0}",
            new Dictionary<string, string>() {
                {"ko","リモート セッション: {0}"},
                {"fr","Sessions à distance : {0}"},
                {"es","Sesiones remotas: {0}"},
                {"ja","リモート セッション: {0}"},
                {"hi","दूरस्थ सत्र: {0}"},
                {"zh-chs","远程会话：{0}"},
                {"de","Remote-Sitzungen: {0}"}
            }
        },
        {
            "Assistant Update",
            new Dictionary<string, string>() {
                {"ko","アシスタントアップデート"},
                {"fr","Mise à jour de l'assistant"},
                {"es","Asistente de actualización"},
                {"ja","アシスタントアップデート"},
                {"hi","सहायक अद्यतन"},
                {"zh-chs","助理更新"},
                {"de","Assistant-Update"}
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
            "{0} remote sessions",
            new Dictionary<string, string>() {
                {"ko","{0} リモート セッション"},
                {"fr","{0} sessions à distance"},
                {"es","{0} sesiones remotas"},
                {"ja","{0} リモート セッション"},
                {"hi","{0} दूरस्थ सत्र"},
                {"zh-chs","{0} 个远程会话"},
                {"de","{0} Remote-Sitzungen"}
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
            "Enter help request details",
            new Dictionary<string, string>() {
                {"ko","ヘルプリクエストの詳細を入力してください"},
                {"fr","Entrez les détails de la demande d'aide"},
                {"es","Ingrese los detalles de la solicitud de ayuda"},
                {"ja","ヘルプリクエストの詳細を入力してください"},
                {"hi","सहायता अनुरोध विवरण दर्ज करें"},
                {"zh-chs","输入帮助请求详细信息"},
                {"de","Geben Sie die Details der Hilfeanfrage ein"}
            }
        },
        {
            "{0} Assistant",
            new Dictionary<string, string>() {
                {"ko","{0}アシスタント"},
                {"fr","{0} Assistante"},
                {"es","{0} Asistente"},
                {"ja","{0}アシスタント"},
                {"hi","{0} सहायक"},
                {"zh-chs","{0} 助理"},
                {"de","{0} Assistent"}
            }
        },
        {
            "&Open",
            new Dictionary<string, string>() {
                {"ko","＆開いた"},
                {"fr","&Ouvert"},
                {"es","&Abierto"},
                {"ja","＆開いた"},
                {"hi","&खुला हुआ"},
                {"zh-chs","＆打开"},
                {"de","&Öffnen"}
            }
        },
        {
            "{0} remote sessions are active.",
            new Dictionary<string, string>() {
                {"ko","{0} 個のリモート セッションがアクティブです。"},
                {"fr","{0} sessions à distance sont actives."},
                {"es","{0} sesiones remotas están activas."},
                {"ja","{0} 個のリモート セッションがアクティブです。"},
                {"hi","{0} दूरस्थ सत्र सक्रिय हैं।"},
                {"zh-chs","{0} 个远程会话处于活动状态。"},
                {"de","{0} Remote-Sitzungen sind aktiv."}
            }
        },
        {
            "E&xit",
            new Dictionary<string, string>() {
                {"ko","出口"},
                {"fr","Sortir"},
                {"es","Salida"},
                {"ja","出口"},
                {"hi","बाहर जाएं"},
                {"zh-chs","出口"},
                {"de","Ausgang"}
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
            "Versions",
            new Dictionary<string, string>() {
                {"ko","バージョン"},
                {"es","Versiones"},
                {"ja","バージョン"},
                {"hi","संस्करणों"},
                {"zh-chs","版本"},
                {"de","Versionen"}
            }
        },
        {
            "Agent is start pending",
            new Dictionary<string, string>() {
                {"ko","エージェントは開始保留中です"},
                {"fr","L'agent est en attente de démarrage"},
                {"es","El agente está pendiente de inicio"},
                {"ja","エージェントは開始保留中です"},
                {"hi","एजेंट प्रारंभ लंबित है"},
                {"zh-chs","代理正在启动待处理"},
                {"de","Agent ist Start ausstehend"}
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
            "Connected to server",
            new Dictionary<string, string>() {
                {"ko","サーバーに接続しました"},
                {"fr","Connecté au serveur"},
                {"es","Conectado al servidor"},
                {"ja","サーバーに接続しました"},
                {"hi","सर्वर से जुड़ा"},
                {"zh-chs","连接到服务器"},
                {"de","Mit Server verbunden"}
            }
        },
        {
            "Value",
            new Dictionary<string, string>() {
                {"ko","値"},
                {"fr","Valeur"},
                {"es","Valor"},
                {"ja","値"},
                {"hi","मूल्य"},
                {"zh-chs","价值"},
                {"de","Wert"}
            }
        },
        {
            "List of remote sessions active on this computer.",
            new Dictionary<string, string>() {
                {"ko","このコンピュータでアクティブなリモート セッションのリスト。"},
                {"fr","Liste des sessions distantes actives sur cet ordinateur."},
                {"es","Lista de sesiones remotas activas en esta computadora."},
                {"ja","このコンピュータでアクティブなリモート セッションのリスト。"},
                {"hi","इस कंप्यूटर पर सक्रिय दूरस्थ सत्रों की सूची।"},
                {"zh-chs","此计算机上活动的远程会话列表。"},
                {"de","Liste der auf diesem Computer aktiven Remotesitzungen."}
            }
        },
        {
            "Help Requested",
            new Dictionary<string, string>() {
                {"ko","リクエストされたヘルプ"},
                {"fr","Aide demandée"},
                {"es","Ayuda solicitada"},
                {"ja","リクエストされたヘルプ"},
                {"hi","सहायता मांगी गई"},
                {"zh-chs","请求帮助"},
                {"de","Hilfe angefordert"}
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
            "Show Sessions...",
            new Dictionary<string, string>() {
                {"ko","セッションを表示..."},
                {"fr","Afficher les séances..."},
                {"es","Mostrar sesiones ..."},
                {"ja","セッションを表示..."},
                {"hi","सत्र दिखाएं..."},
                {"zh-chs","显示会话..."},
                {"de","Sitzungen anzeigen..."}
            }
        },
        {
            "PrivacyBarForm",
            new Dictionary<string, string>() {
                {"ko","プライバシーバーフォーム"},
                {"ja","プライバシーバーフォーム"},
                {"hi","गोपनीयताबारफॉर्म"},
                {"zh-chs","隐私栏表格"},
                {"de","DatenschutzBarForm"}
            }
        },
        {
            "Agent is pause pending",
            new Dictionary<string, string>() {
                {"ko","エージェントは一時停止保留中です"},
                {"fr","L'agent est en attente de pause"},
                {"es","El agente está en pausa pendiente"},
                {"ja","エージェントは一時停止保留中です"},
                {"hi","एजेंट रुका हुआ है"},
                {"zh-chs","代理暂停待定"},
                {"de","Agent ist pausiert"}
            }
        },
        {
            "Agent not installed",
            new Dictionary<string, string>() {
                {"ko","エージェントがインストールされていません"},
                {"fr","Agent non installé"},
                {"es","Agente no instalado"},
                {"ja","エージェントがインストールされていません"},
                {"hi","एजेंट स्थापित नहीं"},
                {"zh-chs","未安装代理"},
                {"de","Agent nicht installiert"}
            }
        },
        {
            "No remote sessions",
            new Dictionary<string, string>() {
                {"ko","リモート セッションなし"},
                {"fr","Pas de sessions à distance"},
                {"es","Sin sesiones remotas"},
                {"ja","リモート セッションなし"},
                {"hi","कोई दूरस्थ सत्र नहीं"},
                {"zh-chs","没有远程会话"},
                {"de","Keine Remote-Sitzungen"}
            }
        },
        {
            "No active remote sessions.",
            new Dictionary<string, string>() {
                {"ko","アクティブなリモート セッションはありません。"},
                {"fr","Aucune session à distance active."},
                {"es","No hay sesiones remotas activas."},
                {"ja","アクティブなリモート セッションはありません。"},
                {"hi","कोई सक्रिय दूरस्थ सत्र नहीं।"},
                {"zh-chs","没有活动的远程会话。"},
                {"de","Keine aktiven Remote-Sitzungen."}
            }
        },
        {
            "Connecting",
            new Dictionary<string, string>() {
                {"ko","接続中"},
                {"fr","De liaison"},
                {"es","Conectando"},
                {"ja","接続中"},
                {"hi","कनेक्ट"},
                {"zh-chs","连接"},
                {"de","Anschließen"}
            }
        },
        {
            "Allow",
            new Dictionary<string, string>() {
                {"ko","許可する"},
                {"fr","Permettre"},
                {"es","Permitir"},
                {"ja","許可する"},
                {"hi","अनुमति"},
                {"zh-chs","允许"},
                {"de","ermöglichen"}
            }
        },
        {
            "Show &Events...",
            new Dictionary<string, string>() {
                {"ko","イベントを表示..."},
                {"fr","Afficher les &événements..."},
                {"es","Espectáculos y eventos ..."},
                {"ja","イベントを表示..."},
                {"hi","&घटनाक्रम दिखाएं..."},
                {"zh-chs","显示(&Events)..."},
                {"de","&Veranstaltungen anzeigen..."}
            }
        },
        {
            "A new version of this software is available. Update now?",
            new Dictionary<string, string>() {
                {"ko","このソフトウェアの新しいバージョンが利用可能です。今すぐアップデート？"},
                {"fr","Une nouvelle version de ce logiciel est disponible. Mettez à jour maintenant?"},
                {"es","Hay disponible una nueva versión de este software. ¿Actualizar ahora?"},
                {"ja","このソフトウェアの新しいバージョンが利用可能です。今すぐアップデート？"},
                {"hi","इस सॉफ़्टवेयर का एक नया संस्करण उपलब्ध है। अभी अद्यतन करें?"},
                {"zh-chs","此软件的新版本可用。现在更新？"},
                {"de","Eine neue Version dieser Software ist verfügbar. Jetzt aktualisieren?"}
            }
        },
        {
            "Event",
            new Dictionary<string, string>() {
                {"ko","イベント"},
                {"fr","Événement"},
                {"es","Evento"},
                {"ja","イベント"},
                {"hi","प्रतिस्पर्धा"},
                {"zh-chs","事件"},
                {"de","Veranstaltung"}
            }
        },
        {
            "Deny",
            new Dictionary<string, string>() {
                {"ko","拒否する"},
                {"fr","Refuser"},
                {"es","Negar"},
                {"ja","拒否する"},
                {"hi","मना"},
                {"zh-chs","否定"},
                {"de","Verweigern"}
            }
        },
        {
            "1 remote session",
            new Dictionary<string, string>() {
                {"ko","1 リモート セッション"},
                {"fr","1 séance à distance"},
                {"es","1 sesión remota"},
                {"ja","1 リモート セッション"},
                {"hi","1 दूरस्थ सत्र"},
                {"zh-chs","1 个远程会话"},
                {"de","1 Remote-Sitzung"}
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
            "Direct Connect",
            new Dictionary<string, string>() {
                {"ko","ダイレクトコネクト"},
                {"fr","Connection directe"},
                {"es","Conexión directa"},
                {"ja","ダイレクトコネクト"},
                {"hi","प्रत्यक्ष रूप से कनेक्ट"},
                {"zh-chs","直接联系"},
                {"de","Direkte Verbindung"}
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
            "1 remote session is active.",
            new Dictionary<string, string>() {
                {"ko","1 つのリモート セッションがアクティブです。"},
                {"fr","1 session à distance est active."},
                {"es","1 sesión remota está activa."},
                {"ja","1 つのリモート セッションがアクティブです。"},
                {"hi","1 दूरस्थ सत्र सक्रिय है।"},
                {"zh-chs","1 个远程会话处于活动状态。"},
                {"de","1 Remote-Sitzung ist aktiv."}
            }
        },
        {
            "(None)",
            new Dictionary<string, string>() {
                {"ko","（無し）"},
                {"fr","(Rien)"},
                {"es","(Ninguno)"},
                {"ja","（無し）"},
                {"hi","(कोई नहीं)"},
                {"zh-chs","（没有任何）"},
                {"de","(Keiner)"}
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
            "Multiple Users",
            new Dictionary<string, string>() {
                {"ko","複数のユーザー"},
                {"fr","Utilisateurs multiples"},
                {"es","Múltiples usuarios"},
                {"ja","複数のユーザー"},
                {"hi","एकाधिक उपयोगकर्ता"},
                {"zh-chs","多个用户"},
                {"de","Mehrere Benutzer"}
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
            "&Close",
            new Dictionary<string, string>() {
                {"ko","＆閉じる"},
                {"fr","&Fermer"},
                {"es","&Cerca"},
                {"ja","＆閉じる"},
                {"hi","&बंद करे"},
                {"zh-chs","＆关闭"},
                {"de","&Schließen"}
            }
        },
        {
            "Authenticating",
            new Dictionary<string, string>() {
                {"ko","認証中"},
                {"fr","Authentification"},
                {"es","Autenticando"},
                {"ja","認証中"},
                {"hi","प्रमाणित कर रहा है"},
                {"zh-chs","认证"},
                {"de","Authentifizierung"}
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
            "S&top Agent",
            new Dictionary<string, string>() {
                {"ko","トップエージェント"},
                {"fr","Agent d'arrêt"},
                {"es","Agente s & top"},
                {"ja","トップエージェント"},
                {"hi","शीर्ष एजेंट"},
                {"zh-chs","停止代理 (&T)"},
                {"de","Stop-Agent"}
            }
        },
        {
            "UDP relay",
            new Dictionary<string, string>() {
                {"ko","UDPリレー"},
                {"fr","relais UDP"},
                {"es","Relé UDP"},
                {"ja","UDPリレー"},
                {"hi","यूडीपी रिले"},
                {"zh-chs","UDP中继"},
                {"de","UDP-Relais"}
            }
        },
        {
            "Agent is stopped",
            new Dictionary<string, string>() {
                {"ko","エージェントが停止しました"},
                {"fr","L'agent est arrêté"},
                {"es","El agente está detenido"},
                {"ja","エージェントが停止しました"},
                {"hi","एजेंट रोक दिया गया है"},
                {"zh-chs","代理已停止"},
                {"de","Agent wurde gestoppt"}
            }
        },
        {
            "Intel® Management Engine",
            new Dictionary<string, string>() {
                {"ko","インテル® マネジメント エンジン"},
                {"fr","Moteur de gestion Intel®"},
                {"es","Motor de administración Intel®"},
                {"ja","インテル® マネジメント エンジン"},
                {"hi","इंटेल® प्रबंधन इंजन"},
                {"zh-chs","英特尔® 管理引擎"},
                {"de","Intel® Management-Engine"}
            }
        },
        {
            "Agent Snapshot",
            new Dictionary<string, string>() {
                {"ko","エージェントスナップショット"},
                {"fr","Instantané de l'agent"},
                {"es","Instantánea del agente"},
                {"ja","エージェントスナップショット"},
                {"hi","एजेंट स्नैपशॉट"},
                {"zh-chs","代理快照"},
                {"de","Agenten-Snapshot"}
            }
        },
        {
            "Request Help...",
            new Dictionary<string, string>() {
                {"ko","ヘルプをリクエスト..."},
                {"fr","Demander de l'aide..."},
                {"es","Solicitar ayuda ..."},
                {"ja","ヘルプをリクエスト..."},
                {"hi","मदद का अनुरोध करें..."},
                {"zh-chs","请求帮助..."},
                {"de","Hilfe anfordern..."}
            }
        },
        {
            "O&pen Site...",
            new Dictionary<string, string>() {
                {"ko","ペンサイト(&P)..."},
                {"fr","&Ouvrir le site..."},
                {"es","Sitio de O & pen ..."},
                {"ja","ペンサイト(&P)..."},
                {"hi","साइट खोलें..."},
                {"zh-chs","打开网站 (&P)..."},
                {"de","Website öffnen..."}
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
            "Cancel Help Request",
            new Dictionary<string, string>() {
                {"ko","ヘルプリクエストをキャンセル"},
                {"fr","Annuler la demande d'aide"},
                {"es","Cancelar solicitud de ayuda"},
                {"ja","ヘルプリクエストをキャンセル"},
                {"hi","सहायता अनुरोध रद्द करें"},
                {"zh-chs","取消帮助请求"},
                {"de","Hilfeanfrage abbrechen"}
            }
        },
        {
            "TCP relay",
            new Dictionary<string, string>() {
                {"ko","TCPリレー"},
                {"fr","Relais TCP"},
                {"es","Relé TCP"},
                {"ja","TCPリレー"},
                {"hi","टीसीपी रिले"},
                {"zh-chs","TCP中继"},
                {"de","TCP-Relais"}
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
            "Agent is running",
            new Dictionary<string, string>() {
                {"ko","エージェントは実行中です"},
                {"fr","L'agent est en cours d'exécution"},
                {"es","El agente se está ejecutando"},
                {"ja","エージェントは実行中です"},
                {"hi","एजेंट चल रहा है"},
                {"zh-chs","代理正在运行"},
                {"de","Agent läuft"}
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
