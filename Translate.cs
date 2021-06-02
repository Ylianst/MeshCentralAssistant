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
            "Agent is paused",
            new Dictionary<string, string>() {
                {"ko","エージェントは一時停止しています"},
                {"fr","L'agent est en pause"},
                {"zh-chs","代理已暂停"},
                {"es","El agente está en pausa"},
                {"hi","एजेंट रुका हुआ है"},
                {"de","Agent ist pausiert"}
            }
        },
        {
            "S&top Agent",
            new Dictionary<string, string>() {
                {"ko","トップエージェント"},
                {"fr","Agent d'arrêt"},
                {"zh-chs","停止代理 (&T)"},
                {"es","Agente s & top"},
                {"hi","शीर्ष एजेंट"},
                {"de","Stop-Agent"}
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
            "Remote Sessions...",
            new Dictionary<string, string>() {
                {"ko","リモート セッション..."},
                {"fr","Séances à distance..."},
                {"zh-chs","远程会话..."},
                {"es","Sesiones remotas ..."},
                {"hi","दूरस्थ सत्र..."},
                {"de","Remote-Sitzungen..."}
            }
        },
        {
            "Remote Sessions",
            new Dictionary<string, string>() {
                {"ko","リモート セッション"},
                {"fr","Séances à distance"},
                {"zh-chs","远程会话"},
                {"es","Sesiones remotas"},
                {"hi","दूरस्थ सत्र"},
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
                {"ru","MeshCentral Ассистент"},
                {"hi","मेषकेंद्रीय सहायक"},
                {"de","MeshCentral-Assistent"}
            }
        },
        {
            "Count",
            new Dictionary<string, string>() {
                {"ko","カウント"},
                {"fr","Compter"},
                {"zh-chs","数数"},
                {"es","Contar"},
                {"hi","गिनती"},
                {"de","Anzahl"}
            }
        },
        {
            "Intel® ME State...",
            new Dictionary<string, string>() {
                {"ko","インテル® ME 状態..."},
                {"fr","État Intel® ME..."},
                {"zh-chs","英特尔® ME 状态..."},
                {"es","Estado Intel® ME ..."},
                {"hi","इंटेल® एमई स्टेट..."},
                {"de","Intel® ME-Zustand..."}
            }
        },
        {
            "Later",
            new Dictionary<string, string>() {
                {"ko","後で"},
                {"fr","Plus tard"},
                {"zh-chs","之后"},
                {"es","Mas tarde"},
                {"hi","बाद में"},
                {"de","Später"}
            }
        },
        {
            "Agent is stopped pending",
            new Dictionary<string, string>() {
                {"ko","エージェントが停止中です"},
                {"fr","L'agent est arrêté en attente"},
                {"zh-chs","代理停止等待"},
                {"es","El agente está detenido pendiente"},
                {"hi","एजेंट लंबित है"},
                {"de","Agent ist gestoppt ausstehend"}
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
                {"zh-chs","更新软件 (&S)"},
                {"es","&Actualiza el software"},
                {"hi","&सॉफ्टवेयर अद्यतन करें"},
                {"de","&Software aktualisieren"}
            }
        },
        {
            "Show &Events...",
            new Dictionary<string, string>() {
                {"ko","イベントを表示..."},
                {"fr","Afficher les &événements..."},
                {"zh-chs","显示(&Events)..."},
                {"es","Espectáculos y eventos ..."},
                {"hi","&घटनाक्रम दिखाएं..."},
                {"de","&Veranstaltungen anzeigen..."}
            }
        },
        {
            "Agent is missing",
            new Dictionary<string, string>() {
                {"ko","エージェントが行方不明"},
                {"fr","L'agent est manquant"},
                {"zh-chs","代理不见了"},
                {"es","Falta el agente"},
                {"hi","एजेंट गायब है"},
                {"de","Agent fehlt"}
            }
        },
        {
            "Assistant Update",
            new Dictionary<string, string>() {
                {"ko","アシスタントアップデート"},
                {"fr","Mise à jour de l'assistant"},
                {"zh-chs","助理更新"},
                {"es","Asistente de actualización"},
                {"hi","सहायक अद्यतन"},
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
                {"zh-chs","{0} 个远程会话"},
                {"es","{0} sesiones remotas"},
                {"hi","{0} दूरस्थ सत्र"},
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
            "{0} Assistant",
            new Dictionary<string, string>() {
                {"ko","{0}アシスタント"},
                {"fr","{0} Assistante"},
                {"zh-chs","{0} 助理"},
                {"es","{0} Asistente"},
                {"hi","{0} सहायक"},
                {"de","{0} Assistent"}
            }
        },
        {
            "Connected to server",
            new Dictionary<string, string>() {
                {"ko","サーバーに接続しました"},
                {"fr","Connecté au serveur"},
                {"zh-chs","连接到服务器"},
                {"es","Conectado al servidor"},
                {"hi","सर्वर से जुड़ा"},
                {"de","Mit Server verbunden"}
            }
        },
        {
            "Connecting",
            new Dictionary<string, string>() {
                {"ko","接続中"},
                {"fr","De liaison"},
                {"zh-chs","连接"},
                {"es","Conectando"},
                {"hi","कनेक्ट"},
                {"de","Anschließen"}
            }
        },
        {
            "&Start Agent",
            new Dictionary<string, string>() {
                {"ko","エージェントを開始"},
                {"fr","&Démarrer l'agent"},
                {"zh-chs","启动代理 (&S)"},
                {"es","&Iniciar agente"},
                {"hi","&प्रारंभ एजेंट"},
                {"de","&Agent starten"}
            }
        },
        {
            "{0} remote sessions are active.",
            new Dictionary<string, string>() {
                {"ko","{0} 個のリモート セッションがアクティブです。"},
                {"fr","{0} sessions à distance sont actives."},
                {"zh-chs","{0} 个远程会话处于活动状态。"},
                {"es","{0} sesiones remotas están activas."},
                {"hi","{0} दूरस्थ सत्र सक्रिय हैं।"},
                {"de","{0} Remote-Sitzungen sind aktiv."}
            }
        },
        {
            "E&xit",
            new Dictionary<string, string>() {
                {"ko","出口"},
                {"fr","Sortir"},
                {"zh-chs","出口"},
                {"es","Salida"},
                {"hi","बाहर जाएं"},
                {"de","Ausgang"}
            }
        },
        {
            "Agent is running",
            new Dictionary<string, string>() {
                {"ko","エージェントは実行中です"},
                {"fr","L'agent est en cours d'exécution"},
                {"zh-chs","代理正在运行"},
                {"es","El agente se está ejecutando"},
                {"hi","एजेंट चल रहा है"},
                {"de","Agent läuft"}
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
            "Request Help",
            new Dictionary<string, string>() {
                {"ko","助けを求める"},
                {"fr","Demander de l'aide"},
                {"zh-chs","请求帮助"},
                {"es","Solicitar ayuda"},
                {"hi","मदद का अनुरोध करें"},
                {"de","Hilfe anfordern"}
            }
        },
        {
            "Versions",
            new Dictionary<string, string>() {
                {"ko","バージョン"},
                {"zh-chs","版本"},
                {"es","Versiones"},
                {"hi","संस्करणों"},
                {"de","Versionen"}
            }
        },
        {
            "Agent Select",
            new Dictionary<string, string>() {
                {"ko","エージェントセレクト"},
                {"fr","Sélection d'agent"},
                {"zh-chs","代理选择"},
                {"es","Seleccionar agente"},
                {"hi","एजेंट चुनें"},
                {"de","Agentenauswahl"}
            }
        },
        {
            "Value",
            new Dictionary<string, string>() {
                {"ko","値"},
                {"fr","Valeur"},
                {"zh-chs","价值"},
                {"es","Valor"},
                {"hi","मूल्य"},
                {"de","Wert"}
            }
        },
        {
            "Agent is pause pending",
            new Dictionary<string, string>() {
                {"ko","エージェントは一時停止保留中です"},
                {"fr","L'agent est en attente de pause"},
                {"zh-chs","代理暂停待定"},
                {"es","El agente está en pausa pendiente"},
                {"hi","एजेंट रुका हुआ है"},
                {"de","Agent ist pausiert"}
            }
        },
        {
            "Remote Sessions: {0}",
            new Dictionary<string, string>() {
                {"ko","リモート セッション: {0}"},
                {"fr","Sessions à distance : {0}"},
                {"zh-chs","远程会话：{0}"},
                {"es","Sesiones remotas: {0}"},
                {"hi","दूरस्थ सत्र: {0}"},
                {"de","Remote-Sitzungen: {0}"}
            }
        },
        {
            "Agent is start pending",
            new Dictionary<string, string>() {
                {"ko","エージェントは開始保留中です"},
                {"fr","L'agent est en attente de démarrage"},
                {"zh-chs","代理正在启动待处理"},
                {"es","El agente está pendiente de inicio"},
                {"hi","एजेंट प्रारंभ लंबित है"},
                {"de","Agent ist Start ausstehend"}
            }
        },
        {
            "Intel® Management Engine state for this computer.",
            new Dictionary<string, string>() {
                {"ko","このコンピューターの Intel® Management Engine の状態。"},
                {"fr","État du moteur de gestion Intel® pour cet ordinateur."},
                {"zh-chs","此计算机的英特尔® 管理引擎状态。"},
                {"es","Estado del motor de administración Intel® para este equipo."},
                {"hi","इस कंप्यूटर के लिए Intel® प्रबंधन इंजन स्थिति।"},
                {"de","Status der Intel® Management Engine für diesen Computer."}
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
            "Item",
            new Dictionary<string, string>() {
                {"ko","項目"},
                {"fr","Article"},
                {"zh-chs","物品"},
                {"es","Artículo"},
                {"hi","मद"},
                {"de","Artikel"}
            }
        },
        {
            "List of remote sessions active on this computer.",
            new Dictionary<string, string>() {
                {"ko","このコンピュータでアクティブなリモート セッションのリスト。"},
                {"fr","Liste des sessions distantes actives sur cet ordinateur."},
                {"zh-chs","此计算机上活动的远程会话列表。"},
                {"es","Lista de sesiones remotas activas en esta computadora."},
                {"hi","इस कंप्यूटर पर सक्रिय दूरस्थ सत्रों की सूची।"},
                {"de","Liste der auf diesem Computer aktiven Remotesitzungen."}
            }
        },
        {
            "Help Requested",
            new Dictionary<string, string>() {
                {"ko","リクエストされたヘルプ"},
                {"fr","Aide demandée"},
                {"zh-chs","请求帮助"},
                {"es","Ayuda solicitada"},
                {"hi","सहायता मांगी गई"},
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
                {"zh-chs","显示会话..."},
                {"es","Mostrar sesiones ..."},
                {"hi","सत्र दिखाएं..."},
                {"de","Sitzungen anzeigen..."}
            }
        },
        {
            "No remote sessions",
            new Dictionary<string, string>() {
                {"ko","リモート セッションなし"},
                {"fr","Pas de sessions à distance"},
                {"zh-chs","没有远程会话"},
                {"es","Sin sesiones remotas"},
                {"hi","कोई दूरस्थ सत्र नहीं"},
                {"de","Keine Remote-Sitzungen"}
            }
        },
        {
            "PrivacyBarForm",
            new Dictionary<string, string>() {
                {"ko","プライバシーバーフォーム"},
                {"zh-chs","隐私栏表格"},
                {"hi","गोपनीयताबारफॉर्म"},
                {"de","DatenschutzBarForm"}
            }
        },
        {
            "Agent not installed",
            new Dictionary<string, string>() {
                {"ko","エージェントがインストールされていません"},
                {"fr","Agent non installé"},
                {"zh-chs","未安装代理"},
                {"es","Agente no instalado"},
                {"hi","एजेंट स्थापित नहीं"},
                {"de","Agent nicht installiert"}
            }
        },
        {
            "Enter help request details",
            new Dictionary<string, string>() {
                {"ko","ヘルプリクエストの詳細を入力してください"},
                {"fr","Entrez les détails de la demande d'aide"},
                {"zh-chs","输入帮助请求详细信息"},
                {"es","Ingrese los detalles de la solicitud de ayuda"},
                {"hi","सहायता अनुरोध विवरण दर्ज करें"},
                {"de","Geben Sie die Details der Hilfeanfrage ein"}
            }
        },
        {
            "No active remote sessions.",
            new Dictionary<string, string>() {
                {"ko","アクティブなリモート セッションはありません。"},
                {"fr","Aucune session à distance active."},
                {"zh-chs","没有活动的远程会话。"},
                {"es","No hay sesiones remotas activas."},
                {"hi","कोई सक्रिय दूरस्थ सत्र नहीं।"},
                {"de","Keine aktiven Remote-Sitzungen."}
            }
        },
        {
            "Allow",
            new Dictionary<string, string>() {
                {"ko","許可する"},
                {"fr","Permettre"},
                {"zh-chs","允许"},
                {"es","Permitir"},
                {"hi","अनुमति"},
                {"de","ermöglichen"}
            }
        },
        {
            "Event",
            new Dictionary<string, string>() {
                {"ko","イベント"},
                {"fr","Événement"},
                {"zh-chs","事件"},
                {"es","Evento"},
                {"hi","प्रतिस्पर्धा"},
                {"de","Veranstaltung"}
            }
        },
        {
            "Deny",
            new Dictionary<string, string>() {
                {"ko","拒否する"},
                {"fr","Refuser"},
                {"zh-chs","否定"},
                {"es","Negar"},
                {"hi","मना"},
                {"de","Verweigern"}
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
            "Authenticating",
            new Dictionary<string, string>() {
                {"ko","認証中"},
                {"fr","Authentification"},
                {"zh-chs","认证"},
                {"es","Autenticando"},
                {"hi","प्रमाणित कर रहा है"},
                {"de","Authentifizierung"}
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
            "1 remote session",
            new Dictionary<string, string>() {
                {"ko","1 リモート セッション"},
                {"fr","1 séance à distance"},
                {"zh-chs","1 个远程会话"},
                {"es","1 sesión remota"},
                {"hi","1 दूरस्थ सत्र"},
                {"de","1 Remote-Sitzung"}
            }
        },
        {
            "1 remote session is active.",
            new Dictionary<string, string>() {
                {"ko","1 つのリモート セッションがアクティブです。"},
                {"fr","1 session à distance est active."},
                {"zh-chs","1 个远程会话处于活动状态。"},
                {"es","1 sesión remota está activa."},
                {"hi","1 दूरस्थ सत्र सक्रिय है।"},
                {"de","1 Remote-Sitzung ist aktiv."}
            }
        },
        {
            "(None)",
            new Dictionary<string, string>() {
                {"ko","（無し）"},
                {"fr","(Rien)"},
                {"zh-chs","（没有任何）"},
                {"es","(Ninguno)"},
                {"hi","(कोई नहीं)"},
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
            "Agent is disconnected",
            new Dictionary<string, string>() {
                {"ko","エージェントが切断されました"},
                {"fr","L'agent est déconnecté"},
                {"zh-chs","代理已断开连接"},
                {"es","El agente está desconectado"},
                {"hi","एजेंट डिस्कनेक्ट हो गया है"},
                {"de","Agent ist getrennt"}
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
            "UDP relay",
            new Dictionary<string, string>() {
                {"ko","UDPリレー"},
                {"fr","relais UDP"},
                {"zh-chs","UDP中继"},
                {"es","Relé UDP"},
                {"hi","यूडीपी रिले"},
                {"de","UDP-Relais"}
            }
        },
        {
            "A new version of this software is available. Update now?",
            new Dictionary<string, string>() {
                {"ko","このソフトウェアの新しいバージョンが利用可能です。今すぐアップデート？"},
                {"fr","Une nouvelle version de ce logiciel est disponible. Mettez à jour maintenant?"},
                {"zh-chs","此软件的新版本可用。现在更新？"},
                {"es","Hay disponible una nueva versión de este software. ¿Actualizar ahora?"},
                {"hi","इस सॉफ़्टवेयर का एक नया संस्करण उपलब्ध है। अभी अद्यतन करें?"},
                {"de","Eine neue Version dieser Software ist verfügbar. Jetzt aktualisieren?"}
            }
        },
        {
            "&Open",
            new Dictionary<string, string>() {
                {"ko","＆開いた"},
                {"fr","&Ouvert"},
                {"zh-chs","＆打开"},
                {"es","&Abierto"},
                {"hi","&खुला हुआ"},
                {"de","&Öffnen"}
            }
        },
        {
            "Multiple Users",
            new Dictionary<string, string>() {
                {"ko","複数のユーザー"},
                {"fr","Utilisateurs multiples"},
                {"zh-chs","多个用户"},
                {"es","Múltiples usuarios"},
                {"hi","एकाधिक उपयोगकर्ता"},
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
                {"zh-chs","＆关闭"},
                {"es","&Cerca"},
                {"hi","&बंद करे"},
                {"de","&Schließen"}
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
            "Cancel Help Request",
            new Dictionary<string, string>() {
                {"ko","ヘルプリクエストをキャンセル"},
                {"fr","Annuler la demande d'aide"},
                {"zh-chs","取消帮助请求"},
                {"es","Cancelar solicitud de ayuda"},
                {"hi","सहायता अनुरोध रद्द करें"},
                {"de","Hilfeanfrage abbrechen"}
            }
        },
        {
            "Agent is stopped",
            new Dictionary<string, string>() {
                {"ko","エージェントが停止しました"},
                {"fr","L'agent est arrêté"},
                {"zh-chs","代理已停止"},
                {"es","El agente está detenido"},
                {"hi","एजेंट रोक दिया गया है"},
                {"de","Agent wurde gestoppt"}
            }
        },
        {
            "Intel® Management Engine",
            new Dictionary<string, string>() {
                {"ko","インテル® マネジメント エンジン"},
                {"fr","Moteur de gestion Intel®"},
                {"zh-chs","英特尔® 管理引擎"},
                {"es","Motor de administración Intel®"},
                {"hi","इंटेल® प्रबंधन इंजन"},
                {"de","Intel® Management-Engine"}
            }
        },
        {
            "Agent Snapshot",
            new Dictionary<string, string>() {
                {"ko","エージェントスナップショット"},
                {"fr","Instantané de l'agent"},
                {"zh-chs","代理快照"},
                {"es","Instantánea del agente"},
                {"hi","एजेंट स्नैपशॉट"},
                {"de","Agenten-Snapshot"}
            }
        },
        {
            "Request Help...",
            new Dictionary<string, string>() {
                {"ko","ヘルプをリクエスト..."},
                {"fr","Demander de l'aide..."},
                {"zh-chs","请求帮助..."},
                {"es","Solicitar ayuda ..."},
                {"hi","मदद का अनुरोध करें..."},
                {"de","Hilfe anfordern..."}
            }
        },
        {
            "Direct Connect",
            new Dictionary<string, string>() {
                {"ko","ダイレクトコネクト"},
                {"fr","Connection directe"},
                {"zh-chs","直接联系"},
                {"es","Conexión directa"},
                {"hi","प्रत्यक्ष रूप से कनेक्ट"},
                {"de","Direkte Verbindung"}
            }
        },
        {
            "O&pen Site...",
            new Dictionary<string, string>() {
                {"ko","ペンサイト(&P)..."},
                {"fr","&Ouvrir le site..."},
                {"zh-chs","打开网站 (&P)..."},
                {"es","Sitio de O & pen ..."},
                {"hi","साइट खोलें..."},
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
            "TCP relay",
            new Dictionary<string, string>() {
                {"ko","TCPリレー"},
                {"fr","Relais TCP"},
                {"zh-chs","TCP中继"},
                {"es","Relé TCP"},
                {"hi","टीसीपी रिले"},
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
            "Agent is continue pending",
            new Dictionary<string, string>() {
                {"ko","エージェントは保留中です"},
                {"fr","L'agent est en attente de poursuite"},
                {"zh-chs","代理正在继续等待"},
                {"es","El agente sigue pendiente"},
                {"hi","एजेंट जारी है लंबित"},
                {"de","Agent ist weiterhin ausstehend"}
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
