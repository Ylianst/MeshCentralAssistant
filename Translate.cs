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
                {"nl","Agent is gepauzeerd"},
                {"ko","에이전트가 일시 중지되었습니다."},
                {"fr","L'agent est en pause"},
                {"it","L'agente è in pausa"},
                {"zh-chs","代理已暂停"},
                {"pt-br","Agente está em pausa"},
                {"sv","Agent är pausad"},
                {"es","El agente está en pausa"},
                {"hi","एजेंट रुका हुआ है"},
                {"de","Agent ist pausiert"}
            }
        },
        {
            "S&top Agent",
            new Dictionary<string, string>() {
                {"ko","Stop 에이전트"},
                {"fr","Agent d'arrêt"},
                {"zh-chs","停止代理 (&T)"},
                {"pt-br","S & top agente"},
                {"sv","S & toppagent"},
                {"es","D&etener Agente"},
                {"hi","शीर्ष एजेंट"},
                {"de","Stop-Agent"}
            }
        },
        {
            "Type",
            new Dictionary<string, string>() {
                {"de","Tippen"},
                {"hi","प्रकार"},
                {"zh-cht","類型"},
                {"zh-chs","类型"},
                {"fi","Tyyppi"},
                {"tr","tip"},
                {"cs","Typ"},
                {"ja","タイプ"},
                {"es","Tipo"},
                {"pt","Tipo"},
                {"pt-br","Modelo"},
                {"sv","Typ"},
                {"ko","유형"},
                {"it","Digita"},
                {"ru","Удаленный ввод"}
            }
        },
        {
            "Remote Sessions",
            new Dictionary<string, string>() {
                {"nl","Externe Sessies"},
                {"ko","원격 세션"},
                {"fr","Séances à distance"},
                {"it","Sessioni remote"},
                {"zh-chs","远程会话"},
                {"pt-br","Sessões Remotas"},
                {"sv","Fjärrsessioner"},
                {"es","Sesiones Remotas"},
                {"hi","दूरस्थ सत्र"},
                {"de","Remote-Sitzungen"}
            }
        },
        {
            "MeshCentral Assistant",
            new Dictionary<string, string>() {
                {"ko","MeshCentral 어시스턴트"},
                {"fr","Assistant MeshCentral"},
                {"it","Assistente MeshCentral"},
                {"zh-chs","MeshCentral 助手"},
                {"es","Asistente MeshCentral"},
                {"ru","MeshCentral Ассистент"},
                {"hi","मेषकेंद्रीय सहायक"},
                {"de","MeshCentral-Assistent"}
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
                {"pt-br","Atualizar"},
                {"sv","Uppdatering"},
                {"ko","개조하다"},
                {"it","Aggiornamenti"},
                {"ru","Обновить"}
            }
        },
        {
            "Count",
            new Dictionary<string, string>() {
                {"nl","Aantal"},
                {"ko","카운트"},
                {"fr","Compter"},
                {"it","Contare"},
                {"zh-chs","数数"},
                {"pt-br","Contar"},
                {"sv","Räkna"},
                {"es","Contar"},
                {"hi","गिनती"},
                {"de","Anzahl"}
            }
        },
        {
            "Intel® ME State...",
            new Dictionary<string, string>() {
                {"nl","Intel® ME status..."},
                {"ko","인텔 ® ME 상태 ..."},
                {"fr","État Intel® ME..."},
                {"it","Stato Intel® ME..."},
                {"zh-chs","英特尔® ME 状态..."},
                {"pt-br","Intel® ME State ..."},
                {"sv","Intel® ME State ..."},
                {"es","Estado Intel® ME ..."},
                {"hi","इंटेल® एमई स्टेट..."},
                {"de","Intel® ME-Zustand..."}
            }
        },
        {
            "Later",
            new Dictionary<string, string>() {
                {"ko","나중"},
                {"fr","Plus tard"},
                {"it","Dopo"},
                {"zh-chs","之后"},
                {"pt-br","Mais tarde"},
                {"sv","Senare"},
                {"es","Mas tarde"},
                {"hi","बाद में"},
                {"de","Später"}
            }
        },
        {
            "Agent is stopped pending",
            new Dictionary<string, string>() {
                {"nl","Agent is in behandeling gestopt"},
                {"ko","에이전트가 중지되었습니다."},
                {"fr","L'agent est arrêté en attente"},
                {"it","L'agente è in attesa di essere arrestato"},
                {"zh-chs","代理停止等待"},
                {"pt-br","Agente está parado pendente"},
                {"sv","Agent stoppas i väntan"},
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
                {"pt-br","Carregando..."},
                {"sv","Läser in..."},
                {"ko","불러오는 중 ..."},
                {"it","Caricamento in corso..."},
                {"ru","Загрузка..."}
            }
        },
        {
            "&Update Software",
            new Dictionary<string, string>() {
                {"nl","&Software bijwerken"},
                {"ko","소프트웨어 업데이트"},
                {"fr","&Mettre à jour le logiciel"},
                {"it","&Aggiorna software"},
                {"zh-chs","更新软件 (&S)"},
                {"pt-br","&Atualizar o software"},
                {"sv","&Uppdatera mjukvara"},
                {"es","&Actualizar el software"},
                {"hi","&सॉफ्टवेयर अद्यतन करें"},
                {"de","&Software aktualisieren"}
            }
        },
        {
            "Show &Events...",
            new Dictionary<string, string>() {
                {"nl","&Gebeurtenissen tonen..."},
                {"ko","이벤트 표시 (& E) ..."},
                {"fr","Afficher les &événements..."},
                {"it","Mostra eventi..."},
                {"zh-chs","显示(&Events)..."},
                {"pt-br","Mostrar & eventos ..."},
                {"sv","Visa &Händelser ..."},
                {"es","Espectáculos y eventos ..."},
                {"hi","&घटनाक्रम दिखाएं..."},
                {"de","&Ereignisse anzeigen..."}
            }
        },
        {
            "Agent is missing",
            new Dictionary<string, string>() {
                {"nl","Agent ontbreekt"},
                {"ko","에이전트가 없습니다"},
                {"fr","L'agent est manquant"},
                {"it","Manca l'agente"},
                {"zh-chs","代理不见了"},
                {"pt-br","Agente está faltando"},
                {"sv","Agent saknas"},
                {"es","Falta el agente"},
                {"hi","एजेंट गायब है"},
                {"de","Agent fehlt"}
            }
        },
        {
            "Assistant Update",
            new Dictionary<string, string>() {
                {"ko","어시스턴트 업데이트"},
                {"fr","Mise à jour de l'assistant"},
                {"it","Aggiornamento Assistente "},
                {"zh-chs","助理更新"},
                {"pt-br","Atualização do assistente"},
                {"sv","Assistentuppdatering"},
                {"es","Asistente de Actualización"},
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
                {"pt-br","Do utilizador"},
                {"sv","Användare"},
                {"ko","사용자"},
                {"it","Utente"},
                {"ru","Пользователь"}
            }
        },
        {
            "{0} remote sessions",
            new Dictionary<string, string>() {
                {"nl","{0} externe sessies"},
                {"ko","{0} 원격 세션"},
                {"fr","{0} sessions à distance"},
                {"it","{0} sessioni remote"},
                {"zh-chs","{0} 个远程会话"},
                {"pt-br","{0} sessões remotas"},
                {"sv","{0} fjärrsessioner"},
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
                {"pt-br","Notificar"},
                {"sv","Meddela"},
                {"ko","알림"},
                {"it","Notifica"},
                {"ru","Уведомить"}
            }
        },
        {
            "{0} Assistant",
            new Dictionary<string, string>() {
                {"ko","{0} 어시스턴트"},
                {"fr","{0} Assistante"},
                {"it","{0} Assistente"},
                {"zh-chs","{0} 助理"},
                {"pt-br","{0} assistente"},
                {"sv","{0} Assistent"},
                {"es","{0} Asistente"},
                {"hi","{0} सहायक"},
                {"de","{0} Assistent"}
            }
        },
        {
            "Connected to server",
            new Dictionary<string, string>() {
                {"nl","Verbonden met server"},
                {"ko","서버에 연결됨"},
                {"fr","Connecté au serveur"},
                {"it","Connesso al server"},
                {"zh-chs","连接到服务器"},
                {"pt-br","Conectado ao servidor"},
                {"sv","Ansluten till servern"},
                {"es","Conectado al servidor"},
                {"hi","सर्वर से जुड़ा"},
                {"de","Mit Server verbunden"}
            }
        },
        {
            "Connecting",
            new Dictionary<string, string>() {
                {"nl","Verbinden"},
                {"ko","연결"},
                {"fr","De liaison"},
                {"it","Collegamento"},
                {"zh-chs","连接"},
                {"pt-br","Conectando"},
                {"sv","Ansluter"},
                {"es","Conectando"},
                {"hi","कनेक्ट"},
                {"de","Anschließen"}
            }
        },
        {
            "&Start Agent",
            new Dictionary<string, string>() {
                {"ko","에이전트 시작"},
                {"fr","&Démarrer l'agent"},
                {"it","&Avvia agente"},
                {"zh-chs","启动代理 (&S)"},
                {"pt-br","& Iniciar Agente"},
                {"sv","& Starta agent"},
                {"es","&Iniciar agente"},
                {"hi","&प्रारंभ एजेंट"},
                {"de","&Agent starten"}
            }
        },
        {
            "{0} remote sessions are active.",
            new Dictionary<string, string>() {
                {"nl","{0} sessies op afstand zijn actief.."},
                {"ko","{0} 원격 세션이 활성화되었습니다."},
                {"fr","{0} sessions à distance sont actives."},
                {"it","{0} sessioni remote sono attive."},
                {"zh-chs","{0} 个远程会话处于活动状态。"},
                {"pt-br","{0} sessões remotas estão ativas."},
                {"sv","{0} fjärrsessioner är aktiva."},
                {"es","{0} sesiones remotas están activas."},
                {"hi","{0} दूरस्थ सत्र सक्रिय हैं।"},
                {"de","{0} Remote-Sitzungen sind aktiv."}
            }
        },
        {
            "E&xit",
            new Dictionary<string, string>() {
                {"nl","Sluiten"},
                {"ko","출구"},
                {"fr","Sortir"},
                {"zh-chs","出口"},
                {"pt-br","Saída"},
                {"sv","Utgång"},
                {"es","Salida"},
                {"hi","बाहर जाएं"},
                {"de","Beenden"}
            }
        },
        {
            "Agent is running",
            new Dictionary<string, string>() {
                {"nl","Agent is actief"},
                {"ko","에이전트가 실행 중입니다."},
                {"fr","L'agent est en cours d'exécution"},
                {"it","L'agente è in esecuzione"},
                {"zh-chs","代理正在运行"},
                {"pt-br","Agente está em execução"},
                {"sv","Agent kör"},
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
                {"pt-br","Claro"},
                {"sv","Klar"},
                {"ko","지우기"},
                {"it","Pulire"},
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
                {"pt-br","Tempo"},
                {"sv","Tid"},
                {"ko","시간"},
                {"it","Tempo"},
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
                {"pt-br","Eventos"},
                {"sv","Händelser"},
                {"ko","이벤트"},
                {"it","Eventi"},
                {"ru","События"}
            }
        },
        {
            "Request Help",
            new Dictionary<string, string>() {
                {"nl","Hulp vragen"},
                {"ko","도움 요청"},
                {"fr","Demander de l'aide"},
                {"it","Richiedi aiuto"},
                {"zh-chs","请求帮助"},
                {"pt-br","Solicite ajuda"},
                {"sv","Begär hjälp"},
                {"es","Solicitar Ayuda"},
                {"hi","मदद का अनुरोध करें"},
                {"de","Hilfe anfordern"}
            }
        },
        {
            "Versions",
            new Dictionary<string, string>() {
                {"nl","Versies"},
                {"ko","버전"},
                {"it","Versioni"},
                {"zh-chs","版本"},
                {"pt-br","Versões"},
                {"sv","Versioner"},
                {"es","Versiones"},
                {"hi","संस्करणों"},
                {"de","Versionen"}
            }
        },
        {
            "Disconnect from server and close?",
            new Dictionary<string, string>() {
                {"nl","Verbinding met de server verbreken en afsluiten?"},
                {"fr","Se déconnecter du serveur et fermer ?"},
                {"pt-br","Desconectar do servidor e fechar?"},
                {"sv","Koppla bort från servern och stänga?"},
                {"es","¿Desconectar del servidor y cerrar?"},
                {"de","Serververbindung trennen und beenden?"}
            }
        },
        {
            "Agent Select",
            new Dictionary<string, string>() {
                {"nl","Agent selecteren"},
                {"es","Selección del Agente"}
            }
        },
        {
            "Value",
            new Dictionary<string, string>() {
                {"nl","Waarde"},
                {"ko","값"},
                {"fr","Valeur"},
                {"it","Valore"},
                {"zh-chs","价值"},
                {"pt-br","Valor"},
                {"sv","Värde"},
                {"es","Valor"},
                {"hi","मूल्य"},
                {"de","Wert"}
            }
        },
        {
            "Agent is pause pending",
            new Dictionary<string, string>() {
                {"nl","Agent is gepauzeerd"},
                {"ko","에이전트가 일시 중지 대기 중입니다."},
                {"fr","L'agent est en attente de pause"},
                {"it","L'agente è in attesa di pausa"},
                {"zh-chs","代理暂停待定"},
                {"pt-br","Agente está em pausa pendente"},
                {"sv","Agent är paus i väntan"},
                {"es","El agente está en pausa pendiente"},
                {"hi","एजेंट रुका हुआ है"},
                {"de","Agent ist pausiert"}
            }
        },
        {
            "Agent is start pending",
            new Dictionary<string, string>() {
                {"nl","Agent wacht op start"},
                {"ko","에이전트가 시작 대기 중입니다."},
                {"fr","L'agent est en attente de démarrage"},
                {"it","L'agente è in attesa di avvio"},
                {"zh-chs","代理正在启动待处理"},
                {"pt-br","Agente está pendente de início"},
                {"sv","Agent väntar"},
                {"es","El agente está pendiente de inicio"},
                {"hi","एजेंट प्रारंभ लंबित है"},
                {"de","Agent ist Start ausstehend"}
            }
        },
        {
            "Intel® Management Engine state for this computer.",
            new Dictionary<string, string>() {
                {"nl","Intel® Management Engine status voor deze computer."},
                {"ko","이 컴퓨터의 인텔 ® 관리 엔진 상태입니다."},
                {"fr","État du moteur de gestion Intel® pour cet ordinateur."},
                {"it","Stato Intel® Management Engine per questo computer."},
                {"zh-chs","此计算机的英特尔® 管理引擎状态。"},
                {"pt-br","Estado do Intel® Management Engine para este computador."},
                {"sv","Intel® Management Engine-tillstånd för den här datorn."},
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
                {"pt-br","Desconectado"},
                {"sv","Frånkopplad"},
                {"ko","연결 해제"},
                {"it","Disconnesso"},
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
                {"pt-br","Estado"},
                {"sv","stat"},
                {"ko","상태"},
                {"it","Stato"},
                {"ru","Состояние"}
            }
        },
        {
            "Item",
            new Dictionary<string, string>() {
                {"nl","Artikel"},
                {"ko","안건"},
                {"fr","Article"},
                {"it","Elemento"},
                {"zh-chs","物品"},
                {"sv","Artikel"},
                {"es","Artículo"},
                {"hi","मद"},
                {"de","Artikel"}
            }
        },
        {
            "List of remote sessions active on this computer.",
            new Dictionary<string, string>() {
                {"nl","Lijst met externe sessies die actief zijn op deze computer."},
                {"ko","이 컴퓨터에서 활성화 된 원격 세션 목록입니다."},
                {"fr","Liste des sessions distantes actives sur cet ordinateur."},
                {"it","Elenco delle sessioni remote attive su questo computer."},
                {"zh-chs","此计算机上活动的远程会话列表。"},
                {"pt-br","Lista de sessões remotas ativas neste computador."},
                {"sv","Lista över fjärrsessioner som är aktiva på den här datorn."},
                {"es","Lista de sesiones remotas activas en esta computadora."},
                {"hi","इस कंप्यूटर पर सक्रिय दूरस्थ सत्रों की सूची।"},
                {"de","Liste der auf diesem Computer aktiven Remotesitzungen."}
            }
        },
        {
            "Help Requested",
            new Dictionary<string, string>() {
                {"nl","Hulp gevraagd"},
                {"ko","도움 요청"},
                {"fr","Aide demandée"},
                {"it","Aiuto richiesto"},
                {"zh-chs","请求帮助"},
                {"pt-br","Ajuda Solicitada"},
                {"sv","Hjälp begärd"},
                {"es","Ayuda Solicitada"},
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
                {"pt-br","Habilitado "},
                {"sv","Aktiverad"},
                {"ko","활성화 됨"},
                {"it","Abilitato"},
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
                {"pt-br","arquivos"},
                {"sv","Filer"},
                {"ko","파일"},
                {"it","File"},
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
                {"pt-br","Cancelar"},
                {"sv","Avbryt"},
                {"ko","취소"},
                {"it","Annulla"},
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
                {"pt-br","Conectado"},
                {"sv","Ansluten"},
                {"ko","연결됨"},
                {"it","Collegato"},
                {"ru","Подключено"}
            }
        },
        {
            "Show Sessions...",
            new Dictionary<string, string>() {
                {"nl","Sessies weergeven..."},
                {"ko","세션 표시 ..."},
                {"fr","Afficher les séances..."},
                {"it","Mostra sessioni..."},
                {"zh-chs","显示会话..."},
                {"pt-br","Mostrar sessões ..."},
                {"sv","Visa sessioner ..."},
                {"es","Mostrar Sesiones ..."},
                {"hi","सत्र दिखाएं..."},
                {"de","Sitzungen anzeigen..."}
            }
        },
        {
            "No remote sessions",
            new Dictionary<string, string>() {
                {"nl","Geen externe sessies"},
                {"ko","원격 세션 없음"},
                {"fr","Pas de sessions à distance"},
                {"it","Nessuna sessione remota"},
                {"zh-chs","没有远程会话"},
                {"pt-br","Sem sessões remotas"},
                {"sv","Inga fjärrsessioner"},
                {"es","Sin sesiones remotas"},
                {"hi","कोई दूरस्थ सत्र नहीं"},
                {"de","Keine Remote-Sitzungen"}
            }
        },
        {
            "PrivacyBarForm",
            new Dictionary<string, string>() {
                {"zh-chs","隐私栏表格"},
                {"hi","गोपनीयताबारफॉर्म"},
                {"de","DatenschutzBarForm"}
            }
        },
        {
            "Agent not installed",
            new Dictionary<string, string>() {
                {"nl","Agent is niet geïnstalleerd"},
                {"ko","에이전트가 설치되지 않았습니다."},
                {"fr","Agent non installé"},
                {"it","Agente non installato"},
                {"zh-chs","未安装代理"},
                {"pt-br","Agente não instalado"},
                {"sv","Agent inte installerad"},
                {"es","Agente no instalado"},
                {"hi","एजेंट स्थापित नहीं"},
                {"de","Agent nicht installiert"}
            }
        },
        {
            "Enter help request details",
            new Dictionary<string, string>() {
                {"nl","Voer de details van het hulpverzoek in"},
                {"ko","도움 요청 세부 정보 입력"},
                {"fr","Entrez les détails de la demande d'aide"},
                {"it","Inserisci i dettagli della richiesta di aiuto"},
                {"zh-chs","输入帮助请求详细信息"},
                {"pt-br","Insira os detalhes do pedido de ajuda"},
                {"sv","Ange information om hjälpförfrågan"},
                {"es","Ingresa los detalles de la solicitud de ayuda"},
                {"hi","सहायता अनुरोध विवरण दर्ज करें"},
                {"de","Geben Sie die Details der Hilfeanfrage ein"}
            }
        },
        {
            "No active remote sessions.",
            new Dictionary<string, string>() {
                {"nl","Geen actieve externe sessies."},
                {"ko","활성 원격 세션이 없습니다."},
                {"fr","Aucune session à distance active."},
                {"it","Nessuna sessione remota attiva."},
                {"zh-chs","没有活动的远程会话。"},
                {"pt-br","Nenhuma sessão remota ativa."},
                {"sv","Inga aktiva fjärrsessioner."},
                {"es","No hay sesiones remotas activas."},
                {"hi","कोई सक्रिय दूरस्थ सत्र नहीं।"},
                {"de","Keine aktiven Remote-Sitzungen."}
            }
        },
        {
            "Allow",
            new Dictionary<string, string>() {
                {"nl","Toestaan"},
                {"ko","허용하다"},
                {"fr","Permettre"},
                {"it","Permettere"},
                {"zh-chs","允许"},
                {"pt-br","Permitir"},
                {"sv","Tillåta"},
                {"es","Permitir"},
                {"hi","अनुमति"},
                {"de","Erlauben"}
            }
        },
        {
            "Event",
            new Dictionary<string, string>() {
                {"nl","Gebeurtenis"},
                {"ko","행사"},
                {"fr","Événement"},
                {"it","Evento"},
                {"zh-chs","事件"},
                {"pt-br","Evento"},
                {"sv","Händelse"},
                {"es","Evento"},
                {"hi","प्रतिस्पर्धा"},
                {"de","Ereignis"}
            }
        },
        {
            "Deny",
            new Dictionary<string, string>() {
                {"nl","Weigeren"},
                {"ko","거부"},
                {"fr","Refuser"},
                {"it","Negare"},
                {"zh-chs","否定"},
                {"pt-br","Negar"},
                {"sv","Förneka"},
                {"es","Negar"},
                {"hi","मना"},
                {"de","Verweigern"}
            }
        },
        {
            "{0} - {1}",
            new Dictionary<string, string>() {
                {"ko","{0}-{1}"}
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
                {"pt-br","Perto"},
                {"sv","Stäng"},
                {"ko","닫기"},
                {"it","Chiudere"},
                {"ru","Закрыть"}
            }
        },
        {
            "Authenticating",
            new Dictionary<string, string>() {
                {"nl","Authenticatie"},
                {"ko","인증 중"},
                {"fr","Authentification"},
                {"it","Autenticando"},
                {"zh-chs","认证"},
                {"pt-br","Autenticando"},
                {"sv","Autentiserande"},
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
                {"pt-br","Desabilitado"},
                {"sv","Inaktiverad"},
                {"ko","비활성화"},
                {"it","Disabilitato"},
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
                {"nl","1 externe sessie"},
                {"ko","원격 세션 1 개"},
                {"fr","1 séance à distance"},
                {"it","1 sessione remota"},
                {"zh-chs","1 个远程会话"},
                {"pt-br","1 sessão remota"},
                {"sv","1 fjärrsession"},
                {"es","1 sesión remota"},
                {"hi","1 दूरस्थ सत्र"},
                {"de","1 Remote-Sitzung"}
            }
        },
        {
            "1 remote session is active.",
            new Dictionary<string, string>() {
                {"nl","1 externe sessie is actief."},
                {"ko","1 개의 원격 세션이 활성화되었습니다."},
                {"fr","1 session à distance est active."},
                {"it","1 sessione remota è attiva."},
                {"zh-chs","1 个远程会话处于活动状态。"},
                {"pt-br","1 sessão remota está ativa."},
                {"sv","En fjärrsession är aktiv."},
                {"es","1 sesión remota está activa."},
                {"hi","1 दूरस्थ सत्र सक्रिय है।"},
                {"de","1 Remote-Sitzung ist aktiv."}
            }
        },
        {
            "(None)",
            new Dictionary<string, string>() {
                {"nl","(Geen)"},
                {"ko","(없음)"},
                {"fr","(Rien)"},
                {"it","(Nessuna)"},
                {"zh-chs","（没有任何）"},
                {"pt-br","(Nenhum)"},
                {"sv","(Ingen)"},
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
                {"pt-br","Área de Trabalho"},
                {"sv","Skrivbord"},
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
                {"pt-br","Desconhecido"},
                {"sv","Okänd"},
                {"ko","알 수 없는"},
                {"it","Sconosciuto"},
                {"ru","Неизвестно"}
            }
        },
        {
            "Agent is disconnected",
            new Dictionary<string, string>() {
                {"nl","Verbinding met agent is verbroken"},
                {"ko","에이전트 연결이 끊어졌습니다."},
                {"fr","L'agent est déconnecté"},
                {"it","L'agente è disconnesso"},
                {"zh-chs","代理已断开连接"},
                {"pt-br","Agente está desconectado"},
                {"sv","Agent är frånkopplad"},
                {"es","El agente está desconectado"},
                {"hi","एजेंट डिस्कनेक्ट हो गया है"},
                {"de","Agent ist getrennt"}
            }
        },
        {
            "Activated",
            new Dictionary<string, string>() {
                {"de","Aktiviert"},
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
                {"pt-br","ativado"},
                {"sv","Aktiverad"},
                {"ko","활성화 됨"},
                {"it","Attivato"},
                {"ru","Активировано"}
            }
        },
        {
            "UDP relay",
            new Dictionary<string, string>() {
                {"nl","UDP relais"},
                {"ko","UDP 릴레이"},
                {"fr","relais UDP"},
                {"it","Rilancio UDP"},
                {"zh-chs","UDP中继"},
                {"pt-br","Retransmissão UDP"},
                {"sv","UDP-relä"},
                {"es","Relé UDP"},
                {"hi","यूडीपी रिले"},
                {"de","UDP-Relais"}
            }
        },
        {
            "A new version of this software is available. Update now?",
            new Dictionary<string, string>() {
                {"nl","Er is een nieuwe versie van deze software beschikbaar. Nu bijwerken?"},
                {"ko","이 소프트웨어의 새 버전을 사용할 수 있습니다. 지금 업데이트 하시겠습니까?"},
                {"fr","Une nouvelle version de ce logiciel est disponible. Mettez à jour maintenant?"},
                {"it","È disponibile una nuova versione di questo software. Aggiorna ora?"},
                {"zh-chs","此软件的新版本可用。现在更新？"},
                {"pt-br","Uma nova versão deste software está disponível. Atualizar agora?"},
                {"sv","En ny version av denna programvara är tillgänglig. Uppdatera nu?"},
                {"es","Hay disponible una nueva versión de este software. ¿Actualizar ahora?"},
                {"hi","इस सॉफ़्टवेयर का एक नया संस्करण उपलब्ध है। अभी अद्यतन करें?"},
                {"de","Eine neue Version dieser Software ist verfügbar. Jetzt aktualisieren?"}
            }
        },
        {
            "&Open",
            new Dictionary<string, string>() {
                {"ko","&열다"},
                {"fr","&Ouvert"},
                {"it","&Apri"},
                {"zh-chs","＆打开"},
                {"pt-br","&Aberto"},
                {"sv","&Öppna"},
                {"es","&Abierto"},
                {"hi","&खुला हुआ"},
                {"de","&Öffnen"}
            }
        },
        {
            "Multiple Users",
            new Dictionary<string, string>() {
                {"nl","Meerdere gebruikers"},
                {"ko","여러 사용자"},
                {"fr","Utilisateurs multiples"},
                {"it","Utenti multipli"},
                {"zh-chs","多个用户"},
                {"pt-br","Múltiplos usuários"},
                {"sv","Flera användare"},
                {"es","Múltiples Usuarios"},
                {"hi","एकाधिक उपयोगकर्ता"},
                {"de","Mehrere Benutzer"}
            }
        },
        {
            "Remote Sessions: {0}",
            new Dictionary<string, string>() {
                {"nl","Externe Sessies: {0}"},
                {"ko","원격 세션 : {0}"},
                {"fr","Sessions à distance : {0}"},
                {"it","Sessioni remote: {0}"},
                {"zh-chs","远程会话：{0}"},
                {"pt-br","Sessões remotas: {0}"},
                {"sv","Fjärrsessioner: {0}"},
                {"es","Sesiones Remotas: {0}"},
                {"hi","दूरस्थ सत्र: {0}"},
                {"de","Remote-Sitzungen: {0}"}
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
                {"pt-br","terminal"},
                {"ko","터미널"},
                {"it","Terminale"},
                {"ru","Терминал"}
            }
        },
        {
            "&Close",
            new Dictionary<string, string>() {
                {"nl","&Sluiten"},
                {"ko","&닫기"},
                {"fr","&Fermer"},
                {"it","&Chiudi"},
                {"zh-chs","＆关闭"},
                {"pt-br","&Perto"},
                {"sv","&Stäng"},
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
                {"pt-br","Consentimento do usuário"},
                {"sv","Användarens samtycke"},
                {"ko","사용자 연결 옵션"},
                {"it","Consenso dell'utente "},
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
                {"es","No Activada (Pre)"},
                {"pt","Não ativado (pré)"},
                {"nl","Niet geactiveerd (Pre)"},
                {"pt-br","Não ativado (pré)"},
                {"sv","Ej aktiverad (Pre)"},
                {"ko","활성화되지 않음 (Pre)"},
                {"it","Non attivato (pre) "},
                {"ru","Не активированно (Pre)"}
            }
        },
        {
            "Cancel Help Request",
            new Dictionary<string, string>() {
                {"nl","Annuleer hulpverzoek"},
                {"ko","도움말 요청 취소"},
                {"fr","Annuler la demande d'aide"},
                {"it","Annulla la richiesta di aiuto"},
                {"zh-chs","取消帮助请求"},
                {"pt-br","Cancelar pedido de ajuda"},
                {"sv","Avbryt hjälpförfrågan"},
                {"es","Cancelar Solicitud de Ayuda"},
                {"hi","सहायता अनुरोध रद्द करें"},
                {"de","Hilfeanfrage abbrechen"}
            }
        },
        {
            "Agent is stopped",
            new Dictionary<string, string>() {
                {"nl","Agent is gestopt"},
                {"ko","에이전트가 중지되었습니다."},
                {"fr","L'agent est arrêté"},
                {"it","L'agente non è in esecuzione"},
                {"zh-chs","代理已停止"},
                {"pt-br","Agente está parado"},
                {"sv","Agent stoppas"},
                {"es","El agente está detenido"},
                {"hi","एजेंट रोक दिया गया है"},
                {"de","Agent wurde gestoppt"}
            }
        },
        {
            "Intel® Management Engine",
            new Dictionary<string, string>() {
                {"ko","인텔 ® 관리 엔진"},
                {"fr","Moteur de gestion Intel®"},
                {"it","Motore di gestione Intel®"},
                {"zh-chs","英特尔® 管理引擎"},
                {"es","Motor de administración Intel®"},
                {"hi","इंटेल® प्रबंधन इंजन"},
                {"de","Intel® Management-Engine"}
            }
        },
        {
            "Agent Snapshot",
            new Dictionary<string, string>() {
                {"nl","Agent momentopname"},
                {"ko","에이전트 스냅 샷"},
                {"fr","Instantané de l'agent"},
                {"it","Istantanea agente"},
                {"zh-chs","代理快照"},
                {"pt-br","Instantâneo do Agente"},
                {"sv","Agent ögonblicksbild"},
                {"es","Instantánea del agente"},
                {"hi","एजेंट स्नैपशॉट"},
                {"de","Agenten-Snapshot"}
            }
        },
        {
            "Request Help...",
            new Dictionary<string, string>() {
                {"nl","Hulp vragen..."},
                {"ko","도움 요청 ..."},
                {"fr","Demander de l'aide..."},
                {"it","Richiedi Aiuto..."},
                {"zh-chs","请求帮助..."},
                {"pt-br","Solicite ajuda ..."},
                {"sv","Begär hjälp ..."},
                {"es","Solicitar Ayuda ..."},
                {"hi","मदद का अनुरोध करें..."},
                {"de","Hilfe anfordern..."}
            }
        },
        {
            "Direct Connect",
            new Dictionary<string, string>() {
                {"nl","Directe verbinding"},
                {"ko","직접 연결"},
                {"fr","Connection directe"},
                {"it","Collegamento diretto"},
                {"zh-chs","直接联系"},
                {"pt-br","Conexão direta"},
                {"sv","Direktkoppling"},
                {"es","Conexión Directa"},
                {"hi","प्रत्यक्ष रूप से कनेक्ट"},
                {"de","Direkte Verbindung"}
            }
        },
        {
            "O&pen Site...",
            new Dictionary<string, string>() {
                {"nl","O&pen website..."},
                {"ko","O 펜 사이트 (& P) ..."},
                {"fr","&Ouvrir le site..."},
                {"it","O&apri Sito..."},
                {"zh-chs","打开网站 (&P)..."},
                {"pt-br","O & pen Site ..."},
                {"sv","O & pen-webbplats ..."},
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
                {"pt-br","Mandar"},
                {"sv","Skicka"},
                {"ko","전송"},
                {"it","Invia"},
                {"ru","Отправить"}
            }
        },
        {
            "TCP relay",
            new Dictionary<string, string>() {
                {"ko","TCP 릴레이"},
                {"fr","Relais TCP"},
                {"it","Rilancio TCP"},
                {"zh-chs","TCP中继"},
                {"pt-br","Retransmissão TCP"},
                {"sv","TCP-relä"},
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
                {"pt-br","Barra de Privacidade"},
                {"sv","Sekretessfält"},
                {"ko","프라이버시 바"},
                {"it","Privacy bar"},
                {"ru","Панель конфиденциальности"}
            }
        },
        {
            "Agent is continue pending",
            new Dictionary<string, string>() {
                {"nl","Agent is in behandeling"},
                {"ko","에이전트가 계속 대기 중입니다."},
                {"fr","L'agent est en attente de poursuite"},
                {"it","L'agente è ancora in attesa"},
                {"zh-chs","代理正在继续等待"},
                {"pt-br","Agente continua pendente"},
                {"sv","Agent fortsätter i väntan"},
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
                {"pt-br","Não ativado (em)"},
                {"sv","Ej aktiverad (In)"},
                {"ko","활성화되지 않음 (In)"},
                {"it","Non attivato (in) "},
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
                {"pt-br","Console do Agente"},
                {"sv","Agentkonsol"},
                {"ko","에이전트 콘솔"},
                {"it","Console Agente"},
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
