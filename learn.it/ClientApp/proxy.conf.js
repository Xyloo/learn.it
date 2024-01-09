const { env } = require('process');

const PROXY_CONFIG = [
  {
    context: [
      "/Avatars",
      "/AchievementImages",
      "/FlashcardImages"
   ],
    proxyTimeout: 10000,
    target: target,
    secure: "http://localhost:5000",
    headers: {
      Connection: 'Keep-Alive'
    }
  }
]

module.exports = PROXY_CONFIG;
