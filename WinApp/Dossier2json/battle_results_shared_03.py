VEH_CELL_RESULTS = ('health', 'credits', 'xp', 'shots', 'hits', 'thits', 'he_hits', 'pierced', 'damageDealt', 'damageAssistedRadio', 'damageAssistedTrack', 'damageReceived', 'shotsReceived', 'noDamageShotsReceived', 'heHitsReceived', 'piercedReceived', 'spotted', 'damaged', 'kills', 'tdamageDealt', 'tkills', 'isTeamKiller', 'capturePoints', 'droppedCapturePoints', 'mileage', 'lifeTime', 'killerID', 'achievements', 'potentialDamageReceived', 'repair', 'freeXP', 'details', 'potentialDamageDealt', 'soloHitsAssisted', 'isEnemyBaseCaptured', 'stucks', 'autoAimedShots', 'presenceTime', 'spot_list', 'damage_list', 'kill_list', 'ammo', 'crewActivityFlags', 'series', 'tkillRating', 'tkillLog', 'destroyedObjects') 
VEH_CELL_RESULTS_INDICES = dict(((x[1], x[0]) for x in enumerate(VEH_CELL_RESULTS))) 
VEH_BASE_RESULTS = VEH_CELL_RESULTS[:VEH_CELL_RESULTS.index('potentialDamageDealt')] + ('accountDBID', 'team', 'typeCompDescr', 'gold', 'deathReason', 'xpPenalty', 'creditsPenalty', 'creditsContributionIn', 'creditsContributionOut', 'eventIndices', 'vehLockTimeFactor', 'misc') + VEH_CELL_RESULTS[VEH_CELL_RESULTS.index('potentialDamageDealt'):] 
VEH_BASE_RESULTS_INDICES = dict(((x[1], x[0]) for x in enumerate(VEH_BASE_RESULTS))) 
VEH_PUBLIC_RESULTS = VEH_BASE_RESULTS[:VEH_BASE_RESULTS.index('xpPenalty')] 
VEH_PUBLIC_RESULTS_INDICES = dict(((x[1], x[0]) for x in enumerate(VEH_PUBLIC_RESULTS))) 
VEH_ACCOUNT_RESULTS = ('originalCredits', 'originalXP', 'originalFreeXP', 'tmenXP', 'eventCredits', 'eventGold', 'eventXP', 'eventFreeXP', 'eventTMenXP', 'autoRepairCost', 'autoLoadCost', 'autoEquipCost', 'isPremium', 'premiumXPFactor10', 'premiumCreditsFactor10', 'dailyXPFactor10', 'aogasFactor10', 'markOfMastery', 'dossierPopUps') 
VEH_ACCOUNT_RESULTS_INDICES = dict(((x[1], x[0]) for x in enumerate(VEH_ACCOUNT_RESULTS))) 
VEH_FULL_RESULTS = VEH_BASE_RESULTS[:VEH_BASE_RESULTS.index('eventIndices')] + VEH_ACCOUNT_RESULTS 
VEH_FULL_RESULTS_INDICES = dict(((x[1], x[0]) for x in enumerate(VEH_FULL_RESULTS))) 
VEH_ACCOUNT_RESULTS_START_INDEX = VEH_FULL_RESULTS_INDICES['originalCredits'] 
PLAYER_INFO = ('name', 'clanDBID', 'clanAbbrev', 'prebattleID', 'team') 
PLAYER_INFO_INDICES = dict(((x[1], x[0]) for x in enumerate(PLAYER_INFO))) 
COMMON_RESULTS = ('arenaTypeID', 'arenaCreateTime', 'winnerTeam', 'finishReason', 'duration', 'bonusType', 'guiType', 'vehLockMode') 
COMMON_RESULTS_INDICES = dict(((x[1], x[0]) for x in enumerate(COMMON_RESULTS))) 
handled = 1