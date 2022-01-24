/*
Copyright 2009-2022 Intel Corporation

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
        // Do not edit the translations below, instead change the master "translate.json" file of MeshCentral.
        // This file is auto-generated from the "translate.json" file.

        // *** TRANSLATION TABLE START ***
        static private Dictionary<string, Dictionary<string, string>> translationTable = new Dictionary<string, Dictionary<string, string>>() {
        {
            "Agent is start pending",
            new Dictionary<string, string>() {
                {"de","Agent ist Start ausstehend"},
                {"hi","एजेंट प्रारंभ लंबित है"},
                {"fr","L'agent est en attente de démarrage"},
                {"zh-chs","代理正在启动待处理"},
                {"fi","Agentti odottaa odottamista"},
                {"tr","Temsilci beklemeye başladı"},
                {"cs","Agent začíná čekat"},
                {"ja","エージェントは保留中です"},
                {"es","El agente está pendiente de inicio"},
                {"pl","Agent jest w trakcie uruchamiania"},
                {"pt","Agente está pendente de início"},
                {"nl","Agent wacht op start"},
                {"pt-br","Agente está pendente de início"},
                {"sv","Agent väntar"},
                {"da","Agenten afventer start"},
                {"ko","에이전트가 시작 대기 중입니다."},
                {"it","L'agente è in attesa di avvio"},
                {"ru","Агент ожидает запуска"}
            }
        },
        {
            "&Open",
            new Dictionary<string, string>() {
                {"de","&Öffnen"},
                {"hi","&खुला हुआ"},
                {"fr","&Ouvert"},
                {"zh-chs","＆打开"},
                {"fi","&Avata"},
                {"tr","&Açık"},
                {"cs","&Otevřeno"},
                {"ja","＆開ける"},
                {"es","&Abierto"},
                {"pl","&Otwórz"},
                {"pt","&Abrir"},
                {"pt-br","&Abrir"},
                {"sv","&Öppna"},
                {"ko","&열다"},
                {"it","&Apri"},
                {"ru","&Открыть"}
            }
        },
        {
            "Intel® Management Engine state for this computer.",
            new Dictionary<string, string>() {
                {"de","Status der Intel® Management Engine für diesen Computer."},
                {"hi","इस कंप्यूटर के लिए Intel® प्रबंधन इंजन स्थिति।"},
                {"fr","État du moteur de gestion Intel® pour cet ordinateur."},
                {"zh-chs","此计算机的英特尔® 管理引擎状态。"},
                {"fi","Intel® Management Engine -tila tälle tietokoneelle."},
                {"tr","Bu bilgisayar için Intel® Yönetim Motoru durumu."},
                {"cs","Stav Intel® Management Engine pro tento počítač."},
                {"ja","このコンピューターのインテル®マネジメント・エンジンの状態。"},
                {"es","Estado del motor de administración Intel® para este equipo."},
                {"pl","Stan Intel® Management Engine tego komputera."},
                {"pt","Estado do Intel® Management Engine para este computador."},
                {"nl","Intel® Management Engine status voor deze computer."},
                {"pt-br","Estado do Intel® Management Engine para este computador."},
                {"sv","Intel® Management Engine-tillstånd för den här datorn."},
                {"da","Intel® Management Engine-tilstand for denne computer."},
                {"ko","이 컴퓨터의 인텔 ® 관리 엔진 상태입니다."},
                {"it","Stato Intel® Management Engine per questo computer."},
                {"ru","Состояние Intel® Management Engine для этого компьютера."}
            }
        },
        {
            "Authenticating",
            new Dictionary<string, string>() {
                {"de","Authentifizierung"},
                {"hi","प्रमाणित कर रहा है"},
                {"fr","Authentification"},
                {"zh-chs","认证"},
                {"fi","Todennetaan"},
                {"tr","kimlik doğrulama"},
                {"cs","Ověřování"},
                {"ja","認証"},
                {"es","Autenticando"},
                {"pl","Uwierzytelnianie"},
                {"pt","Autenticando"},
                {"nl","Authenticatie"},
                {"pt-br","Autenticando"},
                {"sv","Autentiserande"},
                {"da","Godkender"},
                {"ko","인증 중"},
                {"it","Autenticando"},
                {"ru","Аутентификация"}
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
                {"pl","Pliki"},
                {"pt","Arquivos"},
                {"nl","Bestanden"},
                {"pt-br","Arquivos"},
                {"sv","Filer"},
                {"da","Filer"},
                {"ko","파일"},
                {"it","File"},
                {"ru","Файлы"}
            }
        },
        {
            "Agent is continue pending",
            new Dictionary<string, string>() {
                {"de","Agent ist weiterhin ausstehend"},
                {"hi","एजेंट जारी है लंबित"},
                {"fr","L'agent est en attente de poursuite"},
                {"zh-chs","代理正在继续等待"},
                {"fi","Agentti odottaa edelleen"},
                {"tr","Temsilci beklemeye devam ediyor"},
                {"cs","Agent stále čeká na vyřízení"},
                {"ja","エージェントは保留中です"},
                {"es","El agente sigue pendiente"},
                {"pl","Agent jest w trakcie oczekiwania"},
                {"pt","Agente continua pendente"},
                {"nl","Agent is in behandeling"},
                {"pt-br","Agente continua pendente"},
                {"sv","Agent fortsätter i väntan"},
                {"da","Agenten afventer fortsat"},
                {"ko","에이전트가 계속 대기 중입니다."},
                {"it","L'agente è ancora in attesa"},
                {"ru","Агент ожидает продолжения"}
            }
        },
        {
            "Intel® ME State...",
            new Dictionary<string, string>() {
                {"de","Intel® ME-Zustand..."},
                {"hi","इंटेल® एमई स्टेट..."},
                {"fr","État Intel® ME..."},
                {"zh-chs","英特尔® ME 状态..."},
                {"fi","Intel® ME State ..."},
                {"tr","Intel® ME Durumu..."},
                {"cs","Stav Intel® ME ..."},
                {"ja","インテル®MEの状態..."},
                {"es","Estado Intel® ME ..."},
                {"pl","Stan Intel® ME..."},
                {"pt","Intel® ME State ..."},
                {"nl","Intel® ME status..."},
                {"pt-br","Intel® ME State ..."},
                {"sv","Intel® ME State ..."},
                {"da","Intel® ME tilstand..."},
                {"ko","인텔 ® ME 상태 ..."},
                {"it","Stato Intel® ME..."},
                {"ru","Состояние Intel® ME ..."}
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
                {"pl","Pasek Prywatności"},
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
                {"pl","Nie aktywowany (W)"},
                {"pt","Não ativado (entrada)"},
                {"nl","Niet geactiveerd (In)"},
                {"pt-br","Não ativado (In)"},
                {"sv","Ej aktiverad (In)"},
                {"da","Ikke aktiveret (In)"},
                {"ko","활성화되지 않음 (In)"},
                {"it","Non attivato (in) "},
                {"ru","Не активированно (In)"}
            }
        },
        {
            "OK",
            new Dictionary<string, string>() {
                {"hi","ठीक"},
                {"fr","ОК"},
                {"tr","Tamam"},
                {"pt","Ok"},
                {"ko","확인"},
                {"ru","ОК"}
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
                {"pl","Włączono"},
                {"pt","ativado"},
                {"nl","Ingeschakeld"},
                {"pt-br","Habilitado "},
                {"sv","Aktiverad"},
                {"da","Aktiveret"},
                {"ko","활성화 됨"},
                {"it","Abilitato"},
                {"ru","Включено"}
            }
        },
        {
            "1 remote session",
            new Dictionary<string, string>() {
                {"de","1 Remote-Sitzung"},
                {"hi","1 दूरस्थ सत्र"},
                {"fr","1 séance à distance"},
                {"zh-chs","1 个远程会话"},
                {"fi","1 etäistunto"},
                {"tr","1 uzak oturum"},
                {"cs","1 vzdálená relace"},
                {"ja","1つのリモートセッション"},
                {"es","1 sesión remota"},
                {"pl","1 sesja zdalna"},
                {"pt","1 sessão remota"},
                {"nl","1 externe sessie"},
                {"pt-br","1 sessão remota"},
                {"sv","1 fjärrsession"},
                {"da","1 fjernsession"},
                {"ko","원격 세션 1 개"},
                {"it","1 sessione remota"},
                {"ru","1 удаленный сеанс"}
            }
        },
        {
            "Show &Events...",
            new Dictionary<string, string>() {
                {"de","&Ereignisse anzeigen..."},
                {"hi","&घटनाक्रम दिखाएं..."},
                {"fr","Afficher les &événements..."},
                {"zh-chs","显示(&Events)..."},
                {"fi","Näytä ja tapahtumat ..."},
                {"tr","&Etkinlikleri Göster..."},
                {"cs","Show & Events ..."},
                {"ja","＆イベントを表示..."},
                {"es","Espectáculos y eventos ..."},
                {"pl","Pokaż &Zdarzenia..."},
                {"pt","Mostrar & eventos ..."},
                {"nl","&Gebeurtenissen tonen..."},
                {"pt-br","Mostrar & Eventos ..."},
                {"sv","Visa &Händelser ..."},
                {"da","Vis &Events..."},
                {"ko","이벤트 표시 (& E) ..."},
                {"it","Mostra eventi..."},
                {"ru","Показать и события ..."}
            }
        },
        {
            "MeshCentral Assistant",
            new Dictionary<string, string>() {
                {"de","MeshCentral-Assistent"},
                {"hi","मेषकेंद्रीय सहायक"},
                {"fr","Assistant MeshCentral"},
                {"zh-chs","MeshCentral 助手"},
                {"tr","MeshCentral Yardımcısı"},
                {"cs","Asistent MeshCentral"},
                {"ja","MeshCentralアシスタント"},
                {"es","Asistente MeshCentral"},
                {"pl","Asystent MeshCentral"},
                {"ko","MeshCentral 어시스턴트"},
                {"it","Assistente MeshCentral"},
                {"ru","MeshCentral Ассистент"}
            }
        },
        {
            "Multiple Users",
            new Dictionary<string, string>() {
                {"de","Mehrere Benutzer"},
                {"hi","एकाधिक उपयोगकर्ता"},
                {"fr","Utilisateurs multiples"},
                {"zh-chs","多个用户"},
                {"fi","Useita käyttäjiä"},
                {"tr","Birden Çok Kullanıcı"},
                {"cs","Více uživatelů"},
                {"ja","複数のユーザー"},
                {"es","Múltiples Usuarios"},
                {"pl","Wielu Użytkowników"},
                {"pt","Múltiplos usuários"},
                {"nl","Meerdere gebruikers"},
                {"pt-br","Múltiplos usuários"},
                {"sv","Flera användare"},
                {"da","Flere brugere"},
                {"ko","여러 사용자"},
                {"it","Utenti multipli"},
                {"ru","Несколько пользователей"}
            }
        },
        {
            "{0} - {1}",
            new Dictionary<string, string>() {
                {"ja","{0}-{1}"},
                {"ko","{0}-{1}"}
            }
        },
        {
            "Intel® Management Engine",
            new Dictionary<string, string>() {
                {"de","Intel® Management-Engine"},
                {"hi","इंटेल® प्रबंधन इंजन"},
                {"fr","Moteur de gestion Intel®"},
                {"zh-chs","英特尔® 管理引擎"},
                {"tr","Intel® Yönetim Motoru"},
                {"ja","Intel®管理エンジン"},
                {"es","Motor de administración Intel®"},
                {"ko","인텔 ® 관리 엔진"},
                {"it","Motore di gestione Intel®"}
            }
        },
        {
            "Agent Snapshot",
            new Dictionary<string, string>() {
                {"de","Agenten-Snapshot"},
                {"hi","एजेंट स्नैपशॉट"},
                {"fr","Instantané de l'agent"},
                {"zh-chs","代理快照"},
                {"fi","Agentin tilannekuva"},
                {"tr","Aracı Anlık Görüntüsü"},
                {"cs","Snapshot agenta"},
                {"ja","エージェントスナップショット"},
                {"es","Instantánea del agente"},
                {"pl","Snapshot Agenta"},
                {"pt","Instantâneo do Agente"},
                {"nl","Agent momentopname"},
                {"pt-br","Snapshot do Agente"},
                {"sv","Agent ögonblicksbild"},
                {"ko","에이전트 스냅 샷"},
                {"it","Istantanea agente"},
                {"ru","Снимок агента"}
            }
        },
        {
            "Item",
            new Dictionary<string, string>() {
                {"de","Artikel"},
                {"hi","मद"},
                {"fr","Article"},
                {"zh-chs","物品"},
                {"fi","Tuote"},
                {"tr","Kalem"},
                {"cs","Položka"},
                {"ja","アイテム"},
                {"es","Artículo"},
                {"pl","Element"},
                {"nl","Artikel"},
                {"sv","Artikel"},
                {"da","Post"},
                {"ko","안건"},
                {"it","Elemento"},
                {"ru","Элемент"}
            }
        },
        {
            "Agent Select",
            new Dictionary<string, string>() {
                {"de","Agentenauswahl"},
                {"hi","एजेंट चुनें"},
                {"fr","Choix de l'agent"},
                {"zh-chs","代理选择"},
                {"fi","Agentti Valitse"},
                {"tr","Temsilci Seçimi"},
                {"ja","エージェント選択"},
                {"es","Selección del Agente"},
                {"pl","Wybór agenta"},
                {"nl","Agent selecteren"},
                {"pt-br","Selecionar Agent"},
                {"da","Agent valg"},
                {"ko","에이전트 선택"},
                {"it","Selezione agente"},
                {"ru","Агент Выбрать"}
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
                {"tr","Masaüstü"},
                {"cs","Plocha"},
                {"ja","デスクトップ"},
                {"es","Escritorio"},
                {"pl","Pulpit"},
                {"pt","Área de Trabalho"},
                {"nl","Bureaublad"},
                {"pt-br","Área de Trabalho"},
                {"sv","Skrivbord"},
                {"da","Skrivebord"},
                {"ko","데스크탑"},
                {"ru","Рабочий стол"}
            }
        },
        {
            "Later",
            new Dictionary<string, string>() {
                {"de","Später"},
                {"hi","बाद में"},
                {"fr","Plus tard"},
                {"zh-chs","之后"},
                {"fi","Myöhemmin"},
                {"tr","Daha sonra"},
                {"cs","Později"},
                {"ja","後で"},
                {"es","Mas tarde"},
                {"pl","Później"},
                {"pt","Mais tarde"},
                {"pt-br","Mais tarde"},
                {"sv","Senare"},
                {"da","Senere"},
                {"ko","나중"},
                {"it","Dopo"},
                {"ru","Потом"}
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
                {"pl","Zgoda Użytkownika"},
                {"pt","Consentimento do Usuário"},
                {"nl","Toestemming van gebruiker"},
                {"pt-br","Consentimento do usuário"},
                {"sv","Användarens samtycke"},
                {"da","Brugersamtykke"},
                {"ko","사용자 연결 옵션"},
                {"it","Consenso dell'utente "},
                {"ru","Согласие пользователя"}
            }
        },
        {
            "No active remote sessions.",
            new Dictionary<string, string>() {
                {"de","Keine aktiven Remote-Sitzungen."},
                {"hi","कोई सक्रिय दूरस्थ सत्र नहीं।"},
                {"fr","Aucune session à distance active."},
                {"zh-chs","没有活动的远程会话。"},
                {"fi","Ei aktiivisia etäistuntoja."},
                {"tr","Etkin uzak oturum yok."},
                {"cs","Žádné aktivní vzdálené relace."},
                {"ja","アクティブなリモートセッションはありません。"},
                {"es","No hay sesiones remotas activas."},
                {"pl","Brak aktywnej sesji zdalnej."},
                {"pt","Nenhuma sessão remota ativa."},
                {"nl","Geen actieve externe sessies."},
                {"pt-br","Nenhuma sessão remota ativa."},
                {"sv","Inga aktiva fjärrsessioner."},
                {"da","Ingen aktive fjernsessioner."},
                {"ko","활성 원격 세션이 없습니다."},
                {"it","Nessuna sessione remota attiva."},
                {"ru","Нет активных удаленных сеансов."}
            }
        },
        {
            "Agent is stopped pending",
            new Dictionary<string, string>() {
                {"de","Agent ist gestoppt ausstehend"},
                {"hi","एजेंट लंबित है"},
                {"fr","L'agent est arrêté en attente"},
                {"zh-chs","代理停止等待"},
                {"fi","Agentti on pysäytetty odottamaan"},
                {"tr","Aracı beklemede durduruldu"},
                {"cs","Agent přestal čekat"},
                {"ja","エージェントは保留中停止されます"},
                {"es","El agente está detenido pendiente"},
                {"pl","Agent jest zatrzymywany"},
                {"pt","Agente está parado pendente"},
                {"nl","Agent is in behandeling gestopt"},
                {"pt-br","Agente está parado pendente"},
                {"sv","Agent stoppas i väntan"},
                {"da","Agenten afventer stop"},
                {"ko","에이전트가 중지되었습니다."},
                {"it","L'agente è in attesa di essere arrestato"},
                {"ru","Агент остановлен в ожидании"}
            }
        },
        {
            "Assistant Update",
            new Dictionary<string, string>() {
                {"de","Assistant-Update"},
                {"hi","सहायक अद्यतन"},
                {"fr","Mise à jour de l'assistant"},
                {"zh-chs","助理更新"},
                {"fi","Assistant -päivitys"},
                {"tr","Asistan Güncellemesi"},
                {"cs","Aktualizace Asistenta"},
                {"ja","アシスタントアップデート"},
                {"es","Asistente de Actualización"},
                {"pl","Aktualizacja Asystenta"},
                {"pt","Atualização do assistente"},
                {"pt-br","Atualização do assistente"},
                {"sv","Assistentuppdatering"},
                {"da","Assistent opdatering"},
                {"ko","어시스턴트 업데이트"},
                {"it","Aggiornamento Assistente "},
                {"ru","Ассистент Обновление"}
            }
        },
        {
            "Agent is stopped",
            new Dictionary<string, string>() {
                {"de","Agent wurde gestoppt"},
                {"hi","एजेंट रोक दिया गया है"},
                {"fr","L'agent est arrêté"},
                {"zh-chs","代理已停止"},
                {"fi","Agentti pysäytetään"},
                {"tr","Aracı durduruldu"},
                {"cs","Agent je zastaven"},
                {"ja","エージェントが停止しています"},
                {"es","El agente está detenido"},
                {"pl","Agent jest zatrzymany"},
                {"pt","Agente está parado"},
                {"nl","Agent is gestopt"},
                {"pt-br","Agente está parado"},
                {"sv","Agent stoppas"},
                {"da","Agenten er stoppet"},
                {"ko","에이전트가 중지되었습니다."},
                {"it","L'agente non è in esecuzione"},
                {"ru","Агент остановлен"}
            }
        },
        {
            "&Close",
            new Dictionary<string, string>() {
                {"de","&Schließen"},
                {"hi","&बंद करे"},
                {"fr","&Fermer"},
                {"zh-chs","＆关闭"},
                {"fi","&Kiinni"},
                {"tr","&Kapat"},
                {"cs","&Zavřít"},
                {"ja","＆選ぶ"},
                {"es","&Cerca"},
                {"pl","&Zamknij"},
                {"pt","&Fechar"},
                {"nl","&Sluiten"},
                {"pt-br","&Fechar"},
                {"sv","&Stäng"},
                {"ko","&닫기"},
                {"it","&Chiudi"},
                {"ru","&Закрыть"}
            }
        },
        {
            "Terminal",
            new Dictionary<string, string>() {
                {"hi","टर्मिनल"},
                {"zh-cht","終端機"},
                {"zh-chs","终端"},
                {"fi","Pääte"},
                {"tr","Komut Satırı"},
                {"cs","Terminál"},
                {"ja","ターミナル"},
                {"ko","터미널"},
                {"it","Terminale"},
                {"ru","Терминал"}
            }
        },
        {
            "Deny",
            new Dictionary<string, string>() {
                {"de","Verweigern"},
                {"hi","मना"},
                {"fr","Refuser"},
                {"zh-chs","否定"},
                {"fi","Kieltää"},
                {"tr","İnkar etmek"},
                {"cs","Odmítnout"},
                {"ja","拒否"},
                {"es","Negar"},
                {"pl","Odrzucono"},
                {"pt","Negar"},
                {"nl","Weigeren"},
                {"pt-br","Negar"},
                {"sv","Förneka"},
                {"da","Afslå"},
                {"ko","거부"},
                {"it","Negare"},
                {"ru","Отрицать"}
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
                {"pl","Powiadom"},
                {"pt","Notificar"},
                {"nl","Melden"},
                {"pt-br","Notificar"},
                {"sv","Meddela"},
                {"da","Giv besked"},
                {"ko","알림"},
                {"it","Notifica"},
                {"ru","Уведомить"}
            }
        },
        {
            "No remote sessions",
            new Dictionary<string, string>() {
                {"de","Keine Remote-Sitzungen"},
                {"hi","कोई दूरस्थ सत्र नहीं"},
                {"fr","Pas de sessions à distance"},
                {"zh-chs","没有远程会话"},
                {"fi","Ei etäistuntoja"},
                {"tr","Uzak oturum yok"},
                {"cs","Žádné vzdálené relace"},
                {"ja","リモートセッションはありません"},
                {"es","Sin sesiones remotas"},
                {"pl","Brak sesji zdalnych"},
                {"pt","Sem sessões remotas"},
                {"nl","Geen externe sessies"},
                {"pt-br","Sem sessões remotas"},
                {"sv","Inga fjärrsessioner"},
                {"da","Ingen fjernsessioner"},
                {"ko","원격 세션 없음"},
                {"it","Nessuna sessione remota"},
                {"ru","Нет удаленных сеансов"}
            }
        },
        {
            "{0} remote sessions",
            new Dictionary<string, string>() {
                {"de","{0} Remote-Sitzungen"},
                {"hi","{0} दूरस्थ सत्र"},
                {"fr","{0} sessions à distance"},
                {"zh-chs","{0} 个远程会话"},
                {"fi","{0} etäistuntoa"},
                {"tr","{0} uzak oturum"},
                {"cs","Vzdálené relace: {0}"},
                {"ja","{0}リモートセッション"},
                {"es","{0} sesiones remotas"},
                {"pl","{0} zdalnych sesji"},
                {"pt","{0} sessões remotas"},
                {"nl","{0} externe sessies"},
                {"pt-br","{0} sessões remotas"},
                {"sv","{0} fjärrsessioner"},
                {"da","{0} fjernsessioner"},
                {"ko","{0} 원격 세션"},
                {"it","{0} sessioni remote"},
                {"ru","{0} удаленных сеансов"}
            }
        },
        {
            "{0} Assistant",
            new Dictionary<string, string>() {
                {"de","{0} Assistent"},
                {"hi","{0} सहायक"},
                {"fr","{0} Assistante"},
                {"zh-chs","{0} 助理"},
                {"tr","{0} Asistan"},
                {"cs","{0} Asistent"},
                {"ja","{0}アシスタント"},
                {"es","{0} Asistente"},
                {"pl","{0} Asystent"},
                {"pt","{0} assistente"},
                {"pt-br","{0} Assistente"},
                {"sv","{0} Assistent"},
                {"da","{0} Assistent"},
                {"ko","{0} 어시스턴트"},
                {"it","{0} Assistente"},
                {"ru","{0} Ассистент"}
            }
        },
        {
            "O&pen Site...",
            new Dictionary<string, string>() {
                {"de","Website öffnen..."},
                {"hi","साइट खोलें..."},
                {"fr","&Ouvrir le site..."},
                {"zh-chs","打开网站 (&P)..."},
                {"fi","O & kynä -sivusto ..."},
                {"tr","Siteyi&aç..."},
                {"cs","Web O & pen ..."},
                {"ja","O＆penサイト..."},
                {"es","Sitio de O & pen ..."},
                {"pl","O&twórz Stronę..."},
                {"pt","O & pen Site ..."},
                {"nl","O&pen website..."},
                {"pt-br","A&brir Site ..."},
                {"sv","O & pen-webbplats ..."},
                {"ko","O 펜 사이트 (& P) ..."},
                {"it","O&apri Sito..."},
                {"ru","Открытие сайта ..."}
            }
        },
        {
            "Request Help...",
            new Dictionary<string, string>() {
                {"de","Hilfe anfordern..."},
                {"hi","मदद का अनुरोध करें..."},
                {"fr","Demander de l'aide..."},
                {"zh-chs","请求帮助..."},
                {"fi","Pyydä apua ..."},
                {"tr","Yardım İste..."},
                {"cs","Požádat o pomoc ..."},
                {"ja","ヘルプをリクエスト..."},
                {"es","Solicitar Ayuda ..."},
                {"pl","Prośba Pomocy..."},
                {"pt","Solicite ajuda ..."},
                {"nl","Hulp vragen..."},
                {"pt-br","Solicite ajuda ..."},
                {"sv","Begär hjälp ..."},
                {"da","Anmod om hjælp..."},
                {"ko","도움 요청 ..."},
                {"it","Richiedi Aiuto..."},
                {"ru","Запросить помощь ..."}
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
                {"pl","Nie aktywowany (Przed)"},
                {"pt","Não ativado (pré)"},
                {"nl","Niet geactiveerd (Pre)"},
                {"pt-br","Não ativado (pré)"},
                {"sv","Ej aktiverad (Pre)"},
                {"da","Ikke aktiveret (Pre)"},
                {"ko","활성화되지 않음 (Pre)"},
                {"it","Non attivato (pre) "},
                {"ru","Не активированно (Pre)"}
            }
        },
        {
            "PrivacyBarForm",
            new Dictionary<string, string>() {
                {"de","DatenschutzBarForm"},
                {"hi","गोपनीयताबारफॉर्म"},
                {"zh-chs","隐私栏表格"},
                {"tr","GizlilikBarFormu"},
                {"pl","PrivacyBarFormularz"},
                {"ru","Конфиденциальность"}
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
                {"pl","Połączono"},
                {"pt","Conectado"},
                {"nl","Verbonden"},
                {"pt-br","Conectado"},
                {"sv","Ansluten"},
                {"da","Forbundet"},
                {"ko","연결됨"},
                {"it","Collegato"},
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
                {"pl","Zamknij"},
                {"pt","Fechar"},
                {"nl","Sluiten"},
                {"pt-br","Fechar"},
                {"sv","Stäng"},
                {"da","Luk"},
                {"ko","닫기"},
                {"it","Chiudere"},
                {"ru","Закрыть"}
            }
        },
        {
            "1 remote session is active.",
            new Dictionary<string, string>() {
                {"de","1 Remote-Sitzung ist aktiv."},
                {"hi","1 दूरस्थ सत्र सक्रिय है।"},
                {"fr","1 session à distance est active."},
                {"zh-chs","1 个远程会话处于活动状态。"},
                {"fi","1 etäistunto on aktiivinen."},
                {"tr","1 uzak oturum etkin."},
                {"cs","Je aktivní 1 vzdálená relace."},
                {"ja","1つのリモートセッションがアクティブです。"},
                {"es","1 sesión remota está activa."},
                {"pl","1 sesja zdalna jest aktywna."},
                {"pt","1 sessão remota está ativa."},
                {"nl","1 externe sessie is actief."},
                {"pt-br","1 sessão remota está ativa."},
                {"sv","En fjärrsession är aktiv."},
                {"da","En fjernsession er aktiv."},
                {"ko","1 개의 원격 세션이 활성화되었습니다."},
                {"it","1 sessione remota è attiva."},
                {"ru","Активен 1 удаленный сеанс."}
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
                {"pl","Nieznany"},
                {"pt","Desconhecido"},
                {"nl","Onbekend"},
                {"pt-br","Desconhecido"},
                {"sv","Okänd"},
                {"da","Ukendt"},
                {"ko","알 수 없는"},
                {"it","Sconosciuto"},
                {"ru","Неизвестно"}
            }
        },
        {
            "Agent is missing",
            new Dictionary<string, string>() {
                {"de","Agent fehlt"},
                {"hi","एजेंट गायब है"},
                {"fr","L'agent est manquant"},
                {"zh-chs","代理不见了"},
                {"fi","Agentti puuttuu"},
                {"tr","Ajan kayıp"},
                {"cs","Agent chybí"},
                {"ja","エージェントがありません"},
                {"es","Falta el agente"},
                {"pl","Brakuje agenta"},
                {"pt","Agente está faltando"},
                {"nl","Agent ontbreekt"},
                {"pt-br","Agente não encontrado"},
                {"sv","Agent saknas"},
                {"da","Agenten mangler"},
                {"ko","에이전트가 없습니다"},
                {"it","Manca l'agente"},
                {"ru","Агент отсутствует"}
            }
        },
        {
            "TCP relay",
            new Dictionary<string, string>() {
                {"de","TCP-Relais"},
                {"hi","टीसीपी रिले"},
                {"fr","Relais TCP"},
                {"zh-chs","TCP中继"},
                {"fi","TCP -rele"},
                {"tr","TCP rölesi"},
                {"cs","TCP relé"},
                {"ja","TCPリレー"},
                {"es","Relé TCP"},
                {"pl","TCP przekierowanie"},
                {"pt","Retransmissão TCP"},
                {"pt-br","Retransmissão TCP"},
                {"sv","TCP-relä"},
                {"da","TCP-relay"},
                {"ko","TCP 릴레이"},
                {"it","Rilancio TCP"},
                {"ru","Реле TCP"}
            }
        },
        {
            "Allow",
            new Dictionary<string, string>() {
                {"de","Erlauben"},
                {"hi","अनुमति"},
                {"fr","Permettre"},
                {"zh-chs","允许"},
                {"fi","Sallia"},
                {"tr","İzin vermek"},
                {"cs","Dovolit"},
                {"ja","許可する"},
                {"es","Permitir"},
                {"pl","Zezwól"},
                {"pt","Permitir"},
                {"nl","Toestaan"},
                {"pt-br","Permitir"},
                {"sv","Tillåta"},
                {"da","Tillad"},
                {"ko","허용하다"},
                {"it","Permettere"},
                {"ru","Разрешать"}
            }
        },
        {
            "List of remote sessions active on this computer.",
            new Dictionary<string, string>() {
                {"de","Liste der auf diesem Computer aktiven Remotesitzungen."},
                {"hi","इस कंप्यूटर पर सक्रिय दूरस्थ सत्रों की सूची।"},
                {"fr","Liste des sessions distantes actives sur cet ordinateur."},
                {"zh-chs","此计算机上活动的远程会话列表。"},
                {"fi","Luettelo tämän tietokoneen aktiivisista etäistunnoista."},
                {"tr","Bu bilgisayarda etkin olan uzak oturumların listesi."},
                {"cs","Seznam vzdálených relací aktivních na tomto počítači."},
                {"ja","このコンピューターでアクティブなリモートセッションのリスト。"},
                {"es","Lista de sesiones remotas activas en esta computadora."},
                {"pl","Lista sesji zdalnych aktywnych na tym komputerze."},
                {"pt","Lista de sessões remotas ativas neste computador."},
                {"nl","Lijst met externe sessies die actief zijn op deze computer."},
                {"pt-br","Lista de sessões remotas ativas neste computador."},
                {"sv","Lista över fjärrsessioner som är aktiva på den här datorn."},
                {"da","Liste over fjern-sessioner, der er aktive på denne computer."},
                {"ko","이 컴퓨터에서 활성화 된 원격 세션 목록입니다."},
                {"it","Elenco delle sessioni remote attive su questo computer."},
                {"ru","Список удаленных сеансов, активных на этом компьютере."}
            }
        },
        {
            "Remote Sessions",
            new Dictionary<string, string>() {
                {"de","Remote-Sitzungen"},
                {"hi","दूरस्थ सत्र"},
                {"fr","Séances à distance"},
                {"zh-chs","远程会话"},
                {"fi","Etäistunnot"},
                {"tr","Uzak Oturumlar"},
                {"cs","Vzdálené relace"},
                {"ja","リモートセッション"},
                {"es","Sesiones Remotas"},
                {"pl","Sesje Zdalne"},
                {"pt","Sessões Remotas"},
                {"nl","Externe Sessies"},
                {"pt-br","Sessões Remotas"},
                {"sv","Fjärrsessioner"},
                {"da","Fjernsessioner"},
                {"ko","원격 세션"},
                {"it","Sessioni remote"},
                {"ru","Удаленные сеансы"}
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
                {"pl","Wyślij"},
                {"pt","Enviar"},
                {"nl","Verzenden"},
                {"pt-br","Enviar"},
                {"sv","Skicka"},
                {"ko","전송"},
                {"it","Invia"},
                {"ru","Отправить"}
            }
        },
        {
            "Count",
            new Dictionary<string, string>() {
                {"de","Anzahl"},
                {"hi","गिनती"},
                {"fr","Compter"},
                {"zh-chs","数数"},
                {"fi","Kreivi"},
                {"tr","Saymak"},
                {"cs","Počet"},
                {"ja","カウント"},
                {"es","Contar"},
                {"pl","Liczba"},
                {"pt","Contar"},
                {"nl","Aantal"},
                {"pt-br","Contar"},
                {"sv","Räkna"},
                {"da","Beregn"},
                {"ko","카운트"},
                {"it","Contare"},
                {"ru","Считать"}
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
                {"pl","Użytkownik"},
                {"pt","Do utilizador"},
                {"nl","Gebruiker"},
                {"pt-br","Usuário"},
                {"sv","Användare"},
                {"da","Bruger"},
                {"ko","사용자"},
                {"it","Utente"},
                {"ru","Пользователь"}
            }
        },
        {
            "Cancel Help Request",
            new Dictionary<string, string>() {
                {"de","Hilfeanfrage abbrechen"},
                {"hi","सहायता अनुरोध रद्द करें"},
                {"fr","Annuler la demande d'aide"},
                {"zh-chs","取消帮助请求"},
                {"fi","Peruuta Ohjepyyntö"},
                {"tr","Yardım İsteğini İptal Et"},
                {"cs","Zrušit žádost o pomoc"},
                {"ja","ヘルプリクエストをキャンセルする"},
                {"es","Cancelar Solicitud de Ayuda"},
                {"pl","Anuluj Prośbę Pomocy"},
                {"pt","Cancelar pedido de ajuda"},
                {"nl","Annuleer hulpverzoek"},
                {"pt-br","Cancelar pedido de ajuda"},
                {"sv","Avbryt hjälpförfrågan"},
                {"da","Annuller hjælpeanmodning"},
                {"ko","도움말 요청 취소"},
                {"it","Annulla la richiesta di aiuto"},
                {"ru","Отменить запрос помощи"}
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
                {"tr","Agent Komut Satırı"},
                {"cs","Konzole agenta"},
                {"ja","エージェントコンソール"},
                {"es","Consola de Agente"},
                {"pl","Konsola Agenta"},
                {"pt","Console do agente"},
                {"nl","Agent console"},
                {"pt-br","Console do Agente"},
                {"sv","Agentkonsol"},
                {"da","Agent konsol"},
                {"ko","에이전트 콘솔"},
                {"it","Console Agente"},
                {"ru","Консоль агента"}
            }
        },
        {
            "Agent is running",
            new Dictionary<string, string>() {
                {"de","Agent läuft"},
                {"hi","एजेंट चल रहा है"},
                {"fr","L'agent est en cours d'exécution"},
                {"zh-chs","代理正在运行"},
                {"fi","Agentti on käynnissä"},
                {"tr","Aracı çalışıyor"},
                {"cs","Agent běží"},
                {"ja","エージェントが実行されています"},
                {"es","El agente se está ejecutando"},
                {"pl","Agent działa"},
                {"pt","Agente está em execução"},
                {"nl","Agent is actief"},
                {"pt-br","Agente está em execução"},
                {"sv","Agent kör"},
                {"da","Agenten kører"},
                {"ko","에이전트가 실행 중입니다."},
                {"it","L'agente è in esecuzione"},
                {"ru","Агент запущен"}
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
                {"pl","Zdarzenia"},
                {"pt","Eventos"},
                {"nl","Gebeurtenissen"},
                {"pt-br","Eventos"},
                {"sv","Händelser"},
                {"da","Hændelser"},
                {"ko","이벤트"},
                {"it","Eventi"},
                {"ru","События"}
            }
        },
        {
            "Disconnect from server and close?",
            new Dictionary<string, string>() {
                {"de","Serververbindung trennen und beenden?"},
                {"hi","सर्वर से डिस्कनेक्ट करें और बंद करें?"},
                {"fr","Se déconnecter du serveur et fermer ?"},
                {"zh-chs","断开与服务器的连接并关闭？"},
                {"fi","Katkaistaan ​​yhteys palvelimeen ja suljetaan?"},
                {"tr","Sunucuyla bağlantı kesilsin ve kapatılsın mı?"},
                {"cs","Odpojit od serveru a zavřít?"},
                {"ja","サーバーから切断して閉じますか？"},
                {"es","¿Desconectar del servidor y cerrar?"},
                {"pl","Odłączyć się od serwera i zamknąć?"},
                {"pt","Desconectar do servidor e fechar?"},
                {"nl","Verbinding met de server verbreken en afsluiten?"},
                {"pt-br","Desconectar do servidor e fechar?"},
                {"sv","Koppla bort från servern och stänga?"},
                {"da","Afbryd forbindelsen til serveren og lukke?"},
                {"ko","서버에서 연결을 끊고 닫으시겠습니까?"},
                {"it","Disconnettersi dal server e chiudere?"},
                {"ru","Отключиться от сервера и закрыть?"}
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
                {"pl","Czas"},
                {"pt","Tempo"},
                {"nl","Tijd"},
                {"pt-br","Tempo"},
                {"sv","Tid"},
                {"da","Tid"},
                {"ko","시간"},
                {"it","Tempo"},
                {"ru","Время"}
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
                {"pl","Aktualizacja"},
                {"pt","Atualizar"},
                {"nl","Bijwerken"},
                {"pt-br","Atualizar"},
                {"sv","Uppdatering"},
                {"da","Opdatering"},
                {"ko","개조하다"},
                {"it","Aggiornamenti"},
                {"ru","Обновить"}
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
                {"tr","İptal"},
                {"cs","Storno"},
                {"ja","キャンセル"},
                {"es","Cancelar"},
                {"pl","Anuluj"},
                {"pt","Cancelar"},
                {"nl","Annuleren"},
                {"pt-br","Cancelar"},
                {"sv","Avbryt"},
                {"da","Annuller"},
                {"ko","취소"},
                {"it","Annulla"},
                {"ru","Отмена"}
            }
        },
        {
            "Versions",
            new Dictionary<string, string>() {
                {"de","Versionen"},
                {"hi","संस्करणों"},
                {"zh-chs","版本"},
                {"fi","Versiot"},
                {"tr","Sürümler"},
                {"cs","Verze"},
                {"ja","バージョン"},
                {"es","Versiones"},
                {"pl","Wersje"},
                {"pt","Versões"},
                {"nl","Versies"},
                {"pt-br","Versões"},
                {"sv","Versioner"},
                {"da","Versioner"},
                {"ko","버전"},
                {"it","Versioni"},
                {"ru","Версии"}
            }
        },
        {
            "Show Sessions...",
            new Dictionary<string, string>() {
                {"de","Sitzungen anzeigen..."},
                {"hi","सत्र दिखाएं..."},
                {"fr","Afficher les séances..."},
                {"zh-chs","显示会话..."},
                {"fi","Näytä istunnot ..."},
                {"tr","Oturumları Göster..."},
                {"cs","Zobrazit relace ..."},
                {"ja","セッションを表示..."},
                {"es","Mostrar Sesiones ..."},
                {"pl","Pokaż Sesje..."},
                {"pt","Mostrar sessões ..."},
                {"nl","Sessies weergeven..."},
                {"pt-br","Mostrar Sessões ..."},
                {"sv","Visa sessioner ..."},
                {"da","Vis sessioner..."},
                {"ko","세션 표시 ..."},
                {"it","Mostra sessioni..."},
                {"ru","Показать сеансы ..."}
            }
        },
        {
            "Event",
            new Dictionary<string, string>() {
                {"de","Ereignis"},
                {"hi","प्रतिस्पर्धा"},
                {"fr","Événement"},
                {"zh-chs","事件"},
                {"fi","Tapahtuma"},
                {"tr","Etkinlik"},
                {"cs","událost"},
                {"ja","イベント"},
                {"es","Evento"},
                {"pl","Zdarzenie"},
                {"pt","Evento"},
                {"nl","Gebeurtenis"},
                {"pt-br","Evento"},
                {"sv","Händelse"},
                {"da","Hændelse"},
                {"ko","행사"},
                {"it","Evento"},
                {"ru","Мероприятие"}
            }
        },
        {
            "A new version of this software is available. Update now?",
            new Dictionary<string, string>() {
                {"de","Eine neue Version dieser Software ist verfügbar. Jetzt aktualisieren?"},
                {"hi","इस सॉफ़्टवेयर का एक नया संस्करण उपलब्ध है। अभी अद्यतन करें?"},
                {"fr","Une nouvelle version de ce logiciel est disponible. Mettez à jour maintenant?"},
                {"zh-chs","此软件的新版本可用。现在更新？"},
                {"fi","Tästä ohjelmistosta on saatavana uusi versio. Päivitä nyt?"},
                {"tr","Bu yazılımın yeni bir sürümü mevcut. Şimdi güncelle?"},
                {"cs","K dispozici je nová verze tohoto softwaru. Nyní aktualizovat?"},
                {"ja","このソフトウェアの新しいバージョンが利用可能です。今すぐアップデート？"},
                {"es","Hay disponible una nueva versión de este software. ¿Actualizar ahora?"},
                {"pl","Dostępna jest nowa wersja tego oprogramowania. Zaktualizować teraz?"},
                {"pt","Uma nova versão deste software está disponível. Atualizar agora?"},
                {"nl","Er is een nieuwe versie van deze software beschikbaar. Nu bijwerken?"},
                {"pt-br","Uma nova versão deste software está disponível. Deseja atualizar agora?"},
                {"sv","En ny version av denna programvara är tillgänglig. Uppdatera nu?"},
                {"da","En ny version af denne software er tilgængelig. Opdater nu?"},
                {"ko","이 소프트웨어의 새 버전을 사용할 수 있습니다. 지금 업데이트 하시겠습니까?"},
                {"it","È disponibile una nuova versione di questo software. Aggiorna ora?"},
                {"ru","Доступна новая версия этого программного обеспечения. Обновить сейчас?"}
            }
        },
        {
            "Direct Connect",
            new Dictionary<string, string>() {
                {"de","Direkte Verbindung"},
                {"hi","प्रत्यक्ष रूप से कनेक्ट"},
                {"fr","Connection directe"},
                {"zh-chs","直接联系"},
                {"fi","Suora yhteys"},
                {"tr","Doğrudan bağlantı"},
                {"cs","Přímé spojení"},
                {"ja","ダイレクトコネクト"},
                {"es","Conexión Directa"},
                {"pl","Połączenie Bezpośrednie"},
                {"pt","Conexão direta"},
                {"nl","Directe verbinding"},
                {"pt-br","Conexão direta"},
                {"sv","Direktkoppling"},
                {"da","Direkte forbindelse"},
                {"ko","직접 연결"},
                {"it","Collegamento diretto"},
                {"ru","Прямое соединение"}
            }
        },
        {
            "Agent is disconnected",
            new Dictionary<string, string>() {
                {"de","Agent ist getrennt"},
                {"hi","एजेंट डिस्कनेक्ट हो गया है"},
                {"fr","L'agent est déconnecté"},
                {"zh-chs","代理已断开连接"},
                {"fi","Agentti on katkaistu"},
                {"tr","Aracının bağlantısı kesildi"},
                {"cs","Agent je odpojen"},
                {"ja","エージェントが切断されています"},
                {"es","El agente está desconectado"},
                {"pl","Agent jest rozłączony"},
                {"pt","Agente está desconectado"},
                {"nl","Verbinding met agent is verbroken"},
                {"pt-br","Agente está desconectado"},
                {"sv","Agent är frånkopplad"},
                {"da","Agenten er afbrudt"},
                {"ko","에이전트 연결이 끊어졌습니다."},
                {"it","L'agente è disconnesso"},
                {"ru","Агент отключен"}
            }
        },
        {
            "Help Requested",
            new Dictionary<string, string>() {
                {"de","Hilfe angefordert"},
                {"hi","सहायता मांगी गई"},
                {"fr","Aide demandée"},
                {"zh-chs","请求帮助"},
                {"fi","Apua pyydetty"},
                {"tr","Yardım İstendi"},
                {"cs","Požadovaná pomoc"},
                {"ja","ヘルプが要求されました"},
                {"es","Ayuda Solicitada"},
                {"pl","Prośba Pomocy"},
                {"pt","Ajuda Solicitada"},
                {"nl","Hulp gevraagd"},
                {"pt-br","Ajuda Solicitada"},
                {"sv","Hjälp begärd"},
                {"da","Der er anmodet om hjælp"},
                {"ko","도움 요청"},
                {"it","Aiuto richiesto"},
                {"ru","Запрошена помощь"}
            }
        },
        {
            "Connected to server",
            new Dictionary<string, string>() {
                {"de","Mit Server verbunden"},
                {"hi","सर्वर से जुड़ा"},
                {"fr","Connecté au serveur"},
                {"zh-chs","连接到服务器"},
                {"fi","Yhdistetty palvelimeen"},
                {"tr","sunucuya bağlandı"},
                {"cs","Připojeno k serveru"},
                {"ja","サーバーに接続しました"},
                {"es","Conectado al servidor"},
                {"pl","Połączono z serwerem"},
                {"pt","Conectado ao servidor"},
                {"nl","Verbonden met server"},
                {"pt-br","Conectado ao servidor"},
                {"sv","Ansluten till servern"},
                {"da","Forbundet til serveren"},
                {"ko","서버에 연결됨"},
                {"it","Connesso al server"},
                {"ru","Подключено к серверу"}
            }
        },
        {
            "Enter help request details",
            new Dictionary<string, string>() {
                {"de","Geben Sie die Details der Hilfeanfrage ein"},
                {"hi","सहायता अनुरोध विवरण दर्ज करें"},
                {"fr","Entrez les détails de la demande d'aide"},
                {"zh-chs","输入帮助请求详细信息"},
                {"fi","Anna avunpyynnön tiedot"},
                {"tr","Yardım isteği ayrıntılarını girin"},
                {"cs","Zadejte podrobnosti žádosti o pomoc"},
                {"ja","ヘルプリクエストの詳細を入力してください"},
                {"es","Ingresa los detalles de la solicitud de ayuda"},
                {"pl","Wprowadź szczegóły zapytania o pomoc"},
                {"pt","Insira os detalhes do pedido de ajuda"},
                {"nl","Voer de details van het hulpverzoek in"},
                {"pt-br","Insira os detalhes do pedido de ajuda"},
                {"sv","Ange information om hjälpförfrågan"},
                {"da","Indtast oplysninger om hjælpanmodning"},
                {"ko","도움 요청 세부 정보 입력"},
                {"it","Inserisci i dettagli della richiesta di aiuto"},
                {"ru","Введите детали запроса на помощь"}
            }
        },
        {
            "Connecting",
            new Dictionary<string, string>() {
                {"de","Anschließen"},
                {"hi","कनेक्ट"},
                {"fr","De liaison"},
                {"zh-chs","连接"},
                {"fi","Yhdistetään"},
                {"tr","Bağlanıyor"},
                {"cs","Spojovací"},
                {"ja","接続する"},
                {"es","Conectando"},
                {"pl","Łączenie"},
                {"pt","Conectando"},
                {"nl","Verbinden"},
                {"pt-br","Conectando"},
                {"sv","Ansluter"},
                {"da","Forbinder"},
                {"ko","연결"},
                {"it","Collegamento"},
                {"ru","Подключение"}
            }
        },
        {
            "&Update Software",
            new Dictionary<string, string>() {
                {"de","&Software aktualisieren"},
                {"hi","&सॉफ्टवेयर अद्यतन करें"},
                {"fr","&Mettre à jour le logiciel"},
                {"zh-chs","更新软件 (&S)"},
                {"fi","& Päivitä ohjelmisto"},
                {"tr","&Yazılımı Güncelle"},
                {"cs","& Aktualizovat software"},
                {"ja","＆ソフトウェアの更新"},
                {"es","&Actualizar el software"},
                {"pl","&Aktualizuj Oprogramowanie"},
                {"pt","&Atualizar o software"},
                {"nl","&Software bijwerken"},
                {"pt-br","&Atualizar software"},
                {"sv","&Uppdatera mjukvara"},
                {"ko","소프트웨어 업데이트"},
                {"it","&Aggiorna software"},
                {"ru","&Обновить ПО"}
            }
        },
        {
            "Request Help",
            new Dictionary<string, string>() {
                {"de","Hilfe anfordern"},
                {"hi","मदद का अनुरोध करें"},
                {"fr","Demander de l'aide"},
                {"zh-chs","请求帮助"},
                {"fi","Pyydä apua"},
                {"tr","Yardım İste"},
                {"cs","Požádat o pomoc"},
                {"ja","ヘルプをリクエストする"},
                {"es","Solicitar Ayuda"},
                {"pl","Prośba Pomocy"},
                {"pt","Solicite ajuda"},
                {"nl","Hulp vragen"},
                {"pt-br","Solicite ajuda"},
                {"sv","Begär hjälp"},
                {"da","Anmod om hjælp"},
                {"ko","도움 요청"},
                {"it","Richiedi aiuto"},
                {"ru","Запросить помощь"}
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
                {"pl","Stan"},
                {"pt","Estado"},
                {"nl","Status"},
                {"pt-br","Estado"},
                {"sv","stat"},
                {"da","Tilstand"},
                {"ko","상태"},
                {"it","Stato"},
                {"ru","Состояние"}
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
                {"pl","Ładowanie..."},
                {"pt","Carregando..."},
                {"nl","Laden..."},
                {"pt-br","Carregando..."},
                {"sv","Läser in..."},
                {"da","Indlæser..."},
                {"ko","불러오는 중 ..."},
                {"it","Caricamento in corso..."},
                {"ru","Загрузка..."}
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
                {"pl","Rozłączony"},
                {"pt","Desconectado"},
                {"nl","Verbroken"},
                {"pt-br","Desconectado"},
                {"sv","Frånkopplad"},
                {"da","Afbrudt"},
                {"ko","연결 해제"},
                {"it","Disconnesso"},
                {"ru","Отключен"}
            }
        },
        {
            "E&xit",
            new Dictionary<string, string>() {
                {"de","Beenden"},
                {"hi","बाहर जाएं"},
                {"fr","Sortir"},
                {"zh-chs","出口"},
                {"fi","E & xit"},
                {"tr","Çıkış"},
                {"cs","Výstup"},
                {"ja","出口"},
                {"es","Salida"},
                {"pl","Wyjdź (&x)"},
                {"pt","Saída"},
                {"nl","Sluiten"},
                {"pt-br","Saída"},
                {"sv","Utgång"},
                {"ko","출구"},
                {"ru","Выход"}
            }
        },
        {
            "{0} remote sessions are active.",
            new Dictionary<string, string>() {
                {"de","{0} Remote-Sitzungen sind aktiv."},
                {"hi","{0} दूरस्थ सत्र सक्रिय हैं।"},
                {"fr","{0} sessions à distance sont actives."},
                {"zh-chs","{0} 个远程会话处于活动状态。"},
                {"fi","{0} etäistuntoa on aktiivinen."},
                {"tr","{0} uzak oturum etkin."},
                {"cs","Aktivních je {0} vzdálených relací."},
                {"ja","{0}リモートセッションがアクティブです。"},
                {"es","{0} sesiones remotas están activas."},
                {"pl","{0} aktywnych sesji zdalnych."},
                {"pt","{0} sessões remotas estão ativas."},
                {"nl","{0} sessies op afstand zijn actief.."},
                {"pt-br","{0} sessões remotas estão ativas."},
                {"sv","{0} fjärrsessioner är aktiva."},
                {"da","{0} fjernsessioner er aktive."},
                {"ko","{0} 원격 세션이 활성화되었습니다."},
                {"it","{0} sessioni remote sono attive."},
                {"ru","Активных удаленных сеансов: {0}."}
            }
        },
        {
            "S&top Agent",
            new Dictionary<string, string>() {
                {"de","Stop-Agent"},
                {"hi","शीर्ष एजेंट"},
                {"fr","Agent d'arrêt"},
                {"zh-chs","停止代理 (&T)"},
                {"fi","S & ylin agentti"},
                {"tr","Durdurma Aracısı"},
                {"cs","S & top agent"},
                {"ja","S＆topエージェント"},
                {"es","D&etener Agente"},
                {"pl","Zatrzymaj Agenta (S&)"},
                {"pt","S & top agente"},
                {"pt-br","Parar agente"},
                {"sv","S & toppagent"},
                {"ko","Stop 에이전트"},
                {"ru","S & главный агент"}
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
                {"pl","Wyłączone"},
                {"pt","Desativado"},
                {"nl","Uitgeschakeld"},
                {"pt-br","Desabilitado"},
                {"sv","Inaktiverad"},
                {"da","Deaktiveret"},
                {"ko","비활성화"},
                {"it","Disabilitato"},
                {"ru","Отключено"}
            }
        },
        {
            "Agent not installed",
            new Dictionary<string, string>() {
                {"de","Agent nicht installiert"},
                {"hi","एजेंट स्थापित नहीं"},
                {"fr","Agent non installé"},
                {"zh-chs","未安装代理"},
                {"fi","Agenttia ei ole asennettu"},
                {"tr","Aracı yüklenmedi"},
                {"cs","Agent není nainstalován"},
                {"ja","エージェントがインストールされていません"},
                {"es","Agente no instalado"},
                {"pl","Agent nie jest zainstalowany"},
                {"pt","Agente não instalado"},
                {"nl","Agent is niet geïnstalleerd"},
                {"pt-br","Agente não instalado"},
                {"sv","Agent inte installerad"},
                {"da","Agent ikke installeret"},
                {"ko","에이전트가 설치되지 않았습니다."},
                {"it","Agente non installato"},
                {"ru","Агент не установлен"}
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
                {"pl","Aktywowano"},
                {"pt","ativado"},
                {"nl","Geactiveerd"},
                {"pt-br","ativado"},
                {"sv","Aktiverad"},
                {"da","Aktiveret"},
                {"ko","활성화 됨"},
                {"it","Attivato"},
                {"ru","Активировано"}
            }
        },
        {
            "Remote Sessions: {0}",
            new Dictionary<string, string>() {
                {"de","Remote-Sitzungen: {0}"},
                {"hi","दूरस्थ सत्र: {0}"},
                {"fr","Sessions à distance : {0}"},
                {"zh-chs","远程会话：{0}"},
                {"fi","Etäistunnot: {0}"},
                {"tr","Uzak Oturumlar: {0}"},
                {"cs","Vzdálené relace: {0}"},
                {"ja","リモートセッション：{0}"},
                {"es","Sesiones Remotas: {0}"},
                {"pl","Sesje Zdalne: {0}"},
                {"pt","Sessões remotas: {0}"},
                {"nl","Externe Sessies: {0}"},
                {"pt-br","Sessões remotas: {0}"},
                {"sv","Fjärrsessioner: {0}"},
                {"da","Fjernsessioner: {0}"},
                {"ko","원격 세션 : {0}"},
                {"it","Sessioni remote: {0}"},
                {"ru","Удаленные сеансы: {0}"}
            }
        },
        {
            "(None)",
            new Dictionary<string, string>() {
                {"de","(keiner)"},
                {"hi","(कोई नहीं)"},
                {"fr","(Rien)"},
                {"zh-chs","（没有任何）"},
                {"fi","(Ei mitään)"},
                {"tr","(Hiçbiri)"},
                {"cs","(Žádný)"},
                {"ja","（なし）"},
                {"es","(Ninguno)"},
                {"pl","(Żaden)"},
                {"pt","(Nenhum)"},
                {"nl","(Geen)"},
                {"pt-br","(Nenhum)"},
                {"sv","(Ingen)"},
                {"da","(Ingen)"},
                {"ko","(없음)"},
                {"it","(Nessuna)"},
                {"ru","(Никто)"}
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
                {"pl","Wyczyść"},
                {"pt","Limpo"},
                {"nl","Wissen"},
                {"pt-br","Apagar"},
                {"sv","Klar"},
                {"da","Fjern"},
                {"ko","지우기"},
                {"it","Pulire"},
                {"ru","Очистить"}
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
                {"tr","Yazı"},
                {"cs","Typ"},
                {"ja","タイプ"},
                {"es","Tipo"},
                {"pl","Typ"},
                {"pt","Tipo"},
                {"pt-br","Digite"},
                {"sv","Typ"},
                {"ko","유형"},
                {"it","Digita"},
                {"ru","Удаленный ввод"}
            }
        },
        {
            "UDP relay",
            new Dictionary<string, string>() {
                {"de","UDP-Relais"},
                {"hi","यूडीपी रिले"},
                {"fr","relais UDP"},
                {"zh-chs","UDP中继"},
                {"fi","UDP -rele"},
                {"tr","UDP rölesi"},
                {"cs","UDP relé"},
                {"ja","UDPリレー"},
                {"es","Relé UDP"},
                {"pl","Przekierowanie UDP"},
                {"pt","Retransmissão UDP"},
                {"nl","UDP relais"},
                {"pt-br","Retransmissão UDP"},
                {"sv","UDP-relä"},
                {"da","UDP-relay"},
                {"ko","UDP 릴레이"},
                {"it","Rilancio UDP"},
                {"ru","UDP реле"}
            }
        },
        {
            "&Start Agent",
            new Dictionary<string, string>() {
                {"de","&Agent starten"},
                {"hi","&प्रारंभ एजेंट"},
                {"fr","&Démarrer l'agent"},
                {"zh-chs","启动代理 (&S)"},
                {"fi","& Aloita agentti"},
                {"tr","&Başlat Aracısı"},
                {"cs","& Spustit agenta"},
                {"ja","＆Start Agent"},
                {"es","&Iniciar agente"},
                {"pl","&Uruchom Agenta"},
                {"pt","& Iniciar Agente"},
                {"pt-br","&Iniciar Agente"},
                {"sv","& Starta agent"},
                {"da","& Starta agent"},
                {"ko","에이전트 시작"},
                {"it","&Avvia agente"},
                {"ru","&Запустить Агент"}
            }
        },
        {
            "Agent is paused",
            new Dictionary<string, string>() {
                {"de","Agent ist pausiert"},
                {"hi","एजेंट रुका हुआ है"},
                {"fr","L'agent est en pause"},
                {"zh-chs","代理已暂停"},
                {"fi","Agentti on keskeytetty"},
                {"tr","Aracı duraklatıldı"},
                {"cs","Agent je pozastaven"},
                {"ja","エージェントが一時停止しています"},
                {"es","El agente está en pausa"},
                {"pl","Agent jest wstrzymany"},
                {"pt","Agente está em pausa"},
                {"nl","Agent is gepauzeerd"},
                {"pt-br","Agente está em pausa"},
                {"sv","Agent är pausad"},
                {"da","Agenten er sat på pause"},
                {"ko","에이전트가 일시 중지되었습니다."},
                {"it","L'agente è in pausa"},
                {"ru","Агент приостановлен"}
            }
        },
        {
            "Value",
            new Dictionary<string, string>() {
                {"de","Wert"},
                {"hi","मूल्य"},
                {"fr","Valeur"},
                {"zh-chs","价值"},
                {"fi","Arvo"},
                {"tr","Değer"},
                {"cs","Hodnota"},
                {"ja","価値"},
                {"es","Valor"},
                {"pl","Wartość"},
                {"pt","Valor"},
                {"nl","Waarde"},
                {"pt-br","Valor"},
                {"sv","Värde"},
                {"da","Værdi"},
                {"ko","값"},
                {"it","Valore"},
                {"ru","Ценить"}
            }
        },
        {
            "Agent is pause pending",
            new Dictionary<string, string>() {
                {"de","Agent ist pausiert"},
                {"hi","एजेंट रुका हुआ है"},
                {"fr","L'agent est en attente de pause"},
                {"zh-chs","代理暂停待定"},
                {"fi","Agentti odottaa taukoa"},
                {"tr","Aracı duraklatma bekliyor"},
                {"cs","Agent čeká na pozastavení"},
                {"ja","エージェントは一時停止中です"},
                {"es","El agente está en pausa pendiente"},
                {"pl","Agent oczekuje na wstrzymanie"},
                {"pt","Agente está em pausa pendente"},
                {"nl","Agent is gepauzeerd"},
                {"pt-br","Agente está em pausa pendente"},
                {"sv","Agent är paus i väntan"},
                {"da","Agenten afventer pause"},
                {"ko","에이전트가 일시 중지 대기 중입니다."},
                {"it","L'agente è in attesa di pausa"},
                {"ru","Агент ожидает приостановки"}
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
